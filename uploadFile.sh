#!/bin/bash

#1 => domain:port; 2 => secret; 3 => file in your current to upload
curl -H "secret: $2" -w "\n\n%{http_code}\n" -X PUT --progress-bar --data-binary "@$3" "http://""$1""/""$3"
