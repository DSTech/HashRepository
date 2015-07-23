#!/bin/bash

#1 => domain:port; 2 => secret; 3 => file to delete
curl -H "secret: $2" -w "\n\n%{http_code}\n" -X DELETE --progress-bar "http://""$1""/""$3"
