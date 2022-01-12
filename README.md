Hour control is a command line program to help people track they hours to work in a month

# USAGE

Hour control helps you to control better the hours you are going to work by day in the current month
giving to you the amount of hours you need to work per day to reach your final hours in the month

Options: <br />
```
-ho,  --hours             Number of hours you already worked this month   [integer]
-m,   --minutes           Number of minutes you already worked this month [integer]
-th,  --target_hours      Number of target hours you need to work this month (this is 160h by default) [integer]
-iw,  --include_weekend   Use this option if you work at weekends
-e,   --exclude           Number of days to exclude from the month [integer]
-t,   --today             Include the current day in the count
```
# Toggl Integration

Getting data from toggl is possible using the option -tg or --toggl [options] <br />
```
-tg,  --toggl             Use toggl to get Data hours and minutes, this will override the hours and minutes
-tk,  --token             Your token provide by toggl in the account info
-em,  --email             Your email to get the hours from your account
```            
