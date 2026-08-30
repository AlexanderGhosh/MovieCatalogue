#!/bin/bash

# if [[ $EUID -ne 0 ]]; then
    # exec sudo "$0" "$@"
# fi

# sleep 10

DEVICE="/dev/sr0"
BEST_TITLE="1"
PRESET="Fast 1080p30"
OUTPUT="/tmp/tmp $RANDOM.mp4"
KEEP_OUTPUT="false"

while [[ $# -gt 0 ]]; do
    case $1 in
        -s|--subtitle|--subtitles)
            HAS_SUBTITLES=true
            shift
            ;;
        -v)
            VERBOSE=false
            shift
            ;;
        -t|--title)
            BEST_TITLE="$2"
            shift 2
            ;;
        -k|--keep)
            KEEP_OUTPUT="true"
            shift
            ;;
        -p|--preset)
            PRESET="$2"
            shift 2
            ;;
        --tv)
            IS_TV_SHOW="true"
            shift
            ;;
        -l|--length)
            TOTAL_RUNTIME="$2"
            shift 2
            ;;
        -e|--episode-count)
            TOTAL_EPISODES="$2"
            shift 2
            ;;
        -n|--name)
            DVD_NAME="$2"
            shift 2
            ;;
        --no-eject)
            NO_EJECT="true"
            shift
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

[[ -z "$DVD_NAME" ]] && DVD_NAME=$(lsblk -n -o LABEL $DEVICE | tr '[:upper:]' '[:lower:]' | tr '_' ' ' | awk '{for(i=1;i<=NF;i++) $i=toupper(substr($i,1,1)) substr($i,2); print}')

if [[ "$IS_TV_SHOW" == "true" ]]; then
    if [[ -z "${TOTAL_RUNTIME:-}" || -z "${TOTAL_EPISODES:-}" ]]; then
        echo "Error: --tv requires -l/--length and -e/--episode_count" >&2
        exit 1
    fi
    TITLE_FILE="/tmp/tmp $RANDOM.json"
    ARGS=(-i "$DEVICE" -t 0 --scan --json)
    [[ -z "$KEEP_OUTPUT" ]] && ARGS+=(-k)
    HandBrakeCLI -i "$DEVICE" -t 0 --scan --json 2>/dev/null | sed -n '/^JSON Title Set:/,$p' | sed 's/^JSON Title Set: //' > "$TITLE_FILE"
    INDEX=1
    while IFS=$'\t' read -r index name seconds diff; do
        echo "Processing title $index: $name (${seconds}s, off by ${diff}s)"
        EPISODE_NAME="$DVD_NAME E$INDEX"
        "$0" -t "$index" -n "$EPISODE_NAME" -no-eject
        (( INDEX++ ))
    done < <(episode_splitter.sh -f "$TITLE_FILE" -t "$TOTAL_RUNTIME" -e "$TOTAL_EPISODES" -q)
    [[ "$KEEP_OUTPUT" != "true" ]] && rm "$TITLE_FILE"
    [[ "$NO_EJECT" != "true" ]] && eject
    exit 0
fi



ARGS=(-i "$DEVICE" -o "$OUTPUT" --preset="$PRESET" --keep-display-aspect --optimize)

if [[ -z "$BEST_TITLE" ]]; then
    ARGS+=(--main-feature)
else
    ARGS+=(-t "$BEST_TITLE")
fi

if [[ "$HAS_SUBTITLES" == "true" ]]; then
    ARGS+=(--subtitle 1 --subtitle-burned)
else
    ARGS+=(--subtitle none)
fi

[[ "$VERBOSE" = "true" ]] && ARGS+=(-v 1)

HandBrakeCLI "${ARGS[@]}"

if [[ -z "$DVD_NAME" ]]; then
    BODY="{\"Path\":\"$OUTPUT\"}"
else
    BODY="{\"Path\":\"$OUTPUT\",\"Name\":\"$DVD_NAME\"}"
fi

curl localhost/api/video/upload/file -d "$BODY" -H "Content-Type: application/json"
[[ "$KEEP_OUTPUT" != "true" ]] && rm "$OUTPUT"
[[ "$NO_EJECT" != "true" ]] && eject