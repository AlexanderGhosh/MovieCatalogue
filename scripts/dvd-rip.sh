#!/bin/bash

# if [[ $EUID -ne 0 ]]; then
    # exec sudo "$0" "$@"
# fi

# sleep 10

DEVICE="/dev/sr0"
BEST_TITLE="1"
PRESET="Fast 1080p30"
OUTPUT="/tmp/tmp "
OUTPUT+="$RANDOM.mp4"
KEEP_OUTPUT="false"
DVD_NAME=$(lsblk -n -o LABEL /dev/sr0 | tr '[:upper:]' '[:lower:]' | tr '_' ' ' | awk '{for(i=1;i<=NF;i++) $i=toupper(substr($i,1,1)) substr($i,2); print}')

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
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

ARGS=(-i "$DEVICE" -o "$OUTPUT" --preset="$PRESET" --keep-display-aspect)

if [[ -z "$BEST_TITLE" ]]; then
    ARGS+=(-t "$BEST_TITLE")
else
    ARGS+=(--main-feature)
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

curl localhost/api/video/upload/file -d "{\"Path\":\"$OUTPUT\",\"Name\":\"$DVD_NAME\"}" -H "Content-Type: application/json"
[[ "$KEEP_OUTPUT" != "true" ]] && rm "$OUTPUT"
eject