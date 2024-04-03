#!/bin/sh
TEMP_FILE=`mktemp`
clamdscan --fdpass --remove --quiet ${TEMPFILE}/$1 2>${TEMP_FILE}
exit_status=$?

if [ $exit_status -eq 2 ]
then
  echo $exit_status
  cat ${TEMP_FILE} 
else
  echo $exit_status
fi
rm ${TEMP_FILE}