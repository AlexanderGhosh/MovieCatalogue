#!/usr/bin/env bash
#
# find_episodes.sh — Given a HandBrake title-scan JSON file, a total
# back-to-back runtime, and an episode count, echo the titles whose
# individual runtime is within 5 minutes of (total runtime / episode count).
#
# Requires: jq
#
# Usage:
#   ./find_episodes.sh -f titles.json -t TOTAL_RUNTIME -e EPISODE_COUNT [-w TOLERANCE_MIN]
#
# TOTAL_RUNTIME accepts either:
#   - plain minutes, e.g. 220
#   - HH:MM:SS,      e.g. 3:40:00
#
# -w TOLERANCE_MIN (optional) sets the +/- window in minutes. Defaults to 5.
#
# -q (optional) machine-readable mode: suppresses the header/summary lines
#    and prints ONLY one tab-separated record per matching title on stdout:
#       index<TAB>name<TAB>seconds<TAB>diff_seconds
#    This is meant to be looped over from another script, e.g.:
#
#       while IFS=$'\t' read -r index name seconds diff; do
#           echo "Processing title $index: $name ($seconds s)"
#       done < <(./find_episodes.sh -f titles.json -t 3:56:00 -e 8 -q)
#
# Example:
#   ./find_episodes.sh -f titles.json -t 3:56:00 -e 8

set -euo pipefail

TOLERANCE_MIN=5
QUIET=0

usage() {
    echo "Usage: $0 -f <titles.json> -t <total_runtime> -e <episode_count> [-w <tolerance_min>] [-q]" >&2
    echo "  total_runtime: minutes (e.g. 220) or HH:MM:SS (e.g. 3:40:00)" >&2
    echo "  -q: machine-readable mode (tab-separated: index name seconds diff_seconds)" >&2
    exit 1
}

while getopts "f:t:e:w:qh" opt; do
    case "$opt" in
        f) JSON_FILE="$OPTARG" ;;
        t) TOTAL_RUNTIME="$OPTARG" ;;
        e) EPISODE_COUNT="$OPTARG" ;;
        w) TOLERANCE_MIN="$OPTARG" ;;
        q) QUIET=1 ;;
        h) usage ;;
        *) usage ;;
    esac
done

[[ -z "${JSON_FILE:-}" || -z "${TOTAL_RUNTIME:-}" || -z "${EPISODE_COUNT:-}" ]] && usage

command -v jq >/dev/null 2>&1 || { echo "Error: jq is required but not installed." >&2; exit 1; }
[[ -f "$JSON_FILE" ]] || { echo "Error: file not found: $JSON_FILE" >&2; exit 1; }

# --- Parse TOTAL_RUNTIME into seconds ---
if [[ "$TOTAL_RUNTIME" =~ ^[0-9]+:[0-9]{1,2}:[0-9]{1,2}$ ]]; then
    IFS=: read -r h m s <<< "$TOTAL_RUNTIME"
    total_seconds=$(( 10#$h * 3600 + 10#$m * 60 + 10#$s ))
elif [[ "$TOTAL_RUNTIME" =~ ^[0-9]+(\.[0-9]+)?$ ]]; then
    # plain number = minutes
    total_seconds=$(awk -v m="$TOTAL_RUNTIME" 'BEGIN { printf "%d", m * 60 }')
else
    echo "Error: total_runtime must be minutes or HH:MM:SS" >&2
    exit 1
fi

[[ "$EPISODE_COUNT" =~ ^[0-9]+$ && "$EPISODE_COUNT" -gt 0 ]] || { echo "Error: episode_count must be a positive integer" >&2; exit 1; }

target_seconds=$(( total_seconds / EPISODE_COUNT ))
tolerance_seconds=$(( TOLERANCE_MIN * 60 ))

fmt_hms() {
    local total=$1
    printf '%02d:%02d:%02d' $((total/3600)) $(( (total%3600)/60 )) $((total%60))
}

if (( QUIET == 0 )); then
    echo "Total runtime:   $(fmt_hms "$total_seconds")  ($total_seconds s)"
    echo "Episode count:   $EPISODE_COUNT"
    echo "Target/episode:  $(fmt_hms "$target_seconds")  ($target_seconds s)"
    echo "Tolerance:       +/- ${TOLERANCE_MIN} min"
    echo "---"
fi

jq -r '
  .TitleList[]
  | {
      index: .Index,
      name: .Name,
      seconds: ((.Duration.Hours * 3600) + (.Duration.Minutes * 60) + .Duration.Seconds)
    }
  | "\(.index)\t\(.name)\t\(.seconds)"
' "$JSON_FILE" | while IFS=$'\t' read -r index name seconds; do
    diff=$(( seconds - target_seconds ))
    (( diff < 0 )) && diff=$(( -diff ))
    if (( diff <= tolerance_seconds )); then
        if (( QUIET == 1 )); then
            printf '%s\t%s\t%s\t%s\n' "$index" "$name" "$seconds" "$diff"
        else
            echo "Title $index: $name  —  $(fmt_hms "$seconds")  (off by $(fmt_hms "$diff"))"
        fi
    fi
done