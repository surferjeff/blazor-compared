#!/usr/bin/bash
#
# Automatically reruns the app whenever a go source file our template
# source file changes.
#
# Requires https://github.com/watchexec/watchexec

watchexec -e templ -- templ generate & watchexec -r -e go -- go run . &
