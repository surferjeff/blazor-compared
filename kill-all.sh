lsof -i tcp:3000 -i tcp:5126 -i tcp:5085 | grep LISTEN \
    | awk ' { print $2}' | xargs kill