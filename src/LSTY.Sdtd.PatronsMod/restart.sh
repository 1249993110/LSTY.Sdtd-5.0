#!/bin/bash
echo try restart server
count=`ps -ef |grep $1 |grep -v "grep" |wc -l`
if [ $count != 0 ];then
	kill -9 $1
	sleep 3
fi
cd $2
./startserver.sh -configfile=serverconfig.xml
echo "restart complete"
