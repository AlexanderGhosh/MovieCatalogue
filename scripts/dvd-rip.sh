#!/bin/bash

# dvd-rip.sh 
# dvd-rip.sh --tv -e 4 --l 174
# dvd-rip.sh --titles "2,3,4,5"

DEVICE="/dev/sr0"
PRESET="Fast 1080p30"
OUTPUT="/tmp/tmp $RANDOM.mp4"
KEEP_OUTPUT="false"
EPISODE_LENGTH_TOLERANCE="5"
PICKED_EPISODE=0

log() {
    [[ "$VERBOSE" == "true" ]] && echo "$1"
}

while [[ $# -gt 0 ]]; do
    case $1 in
        -s|--subtitle|--subtitles)
            HAS_SUBTITLES=true
            shift
            ;;
        -v)
            VERBOSE="true"
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
        -w|--tolerance)
            EPISODE_LENGTH_TOLERANCE="$2"
            shift 2
            ;;
        --pick-episode)
            PICKED_EPISODE="$2"
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
        --titles)
            TITLE_LIST="$2"
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

[[ -z "$DVD_NAME" ]] && DVD_NAME=$(lsblk -n -o LABEL $DEVICE | tr '[:upper:]' '[:lower:]' | tr '_' ' ' | awk '{for(i=1;i<=NF;i++) $i=toupper(substr($i,1,1)) substr($i,2); print}')

if [[ "$IS_TV_SHOW" == "true" ]]; then
    if [[ -z "${TOTAL_RUNTIME:-}" || -z "${TOTAL_EPISODES:-}" ]]; then
        log "Error: --tv requires -l/--length and -e/--episode-count"
        exit 1
    fi
    log "Is a TV show with total runtime $TOTAL_RUNTIME and total episodes $TOTAL_EPISODES"
    TITLE_FILE="/tmp/tmp $RANDOM.json"
    ARGS=(-i "$DEVICE" -t 0 --scan --json)
    [[ -z "$KEEP_OUTPUT" ]] && ARGS+=(-k)
    HandBrakeCLI "${ARGS[@]}" 2>/dev/null | sed -n '/^JSON Title Set:/,$p' | sed 's/^JSON Title Set: //' > "$TITLE_FILE"
    INDEX=1
    SPLITTER_ARGS=(-f "$TITLE_FILE" -t "$TOTAL_RUNTIME" -e "$TOTAL_EPISODES" -w "$EPISODE_LENGTH_TOLERANCE")
    [[ "$VERBOSE" != "true" ]] && SPLITTER_ARGS+=(-q)
    while IFS=$'\t' read -r title name seconds diff; do
        if [[ "$PICKED_EPISODE" != "0" ]] && [[ "$PICKED_EPISODE" != "$INDEX" ]]; then
            (( INDEX++ ))
            continue
        fi
        log "Processing title $title: $name (${seconds}s, off by ${diff}s)"
        EPISODE_NAME="$DVD_NAME E$INDEX"
        "$0" -t "$title" -n "$EPISODE_NAME" --no-eject < /dev/null
        (( INDEX++ ))
    done < <(episode_splitter.sh "${SPLITTER_ARGS[@]}")
    [[ "$KEEP_OUTPUT" != "true" ]] && rm "$TITLE_FILE"
    [[ "$NO_EJECT" != "true" ]] && eject
    exit 0
fi

if [[ -n "$TITLE_LIST" ]]; then
    IFS="," read -r -a titles <<< "$TITLE_LIST"

    for t in "${numbers[@]}"; do
      "$0" -t "$t" -n "Title: $EPISODE_NAME" --no-eject < /dev/null
    done
    [[ "$KEEP_OUTPUT" != "true" ]] && rm "$TITLE_FILE"
    [[ "$NO_EJECT" != "true" ]] && eject
    exit 0
fi



ARGS=(-i "$DEVICE" -o "$OUTPUT" --preset="$PRESET" --keep-display-aspect --optimize --loose-anamorphic --audio-lang-list "eng")

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