HarmonyHub
==========

C# library for connecting to and controlling the Logitech Harmony Hub.

Status
------

- Authentication to Logitech's web service (myharmony.com) working.
- Authentication to Harmony Hub device working.
- Querying for entire device configuration working.
- Querying for current activity working.
- Start a specific activity (or switch off) working.
- Work in progress: send specific button press

Usage
-----

Check out the HarmonyConsole project to see usage of the HarmonyHub.

Full usage:

      -i, --ip         Required. IP Address of Harmony Hub.
      -u, --user       Required. Logitech Username
      -p, --pass       Required. Logitech Password
      -d, --device     Device ID
      -c, --command    Command to send to device
      -s, --start      Activity ID to start
      -l, --list       List devices or activities
      -g, --get        Get Current Activity
      -o, --off        Turn entire system off
      --help           Display this help screen.

For example, to use the console application to get the current activity on 
the HarmonyHub at IP 10.0.0.1:

    HarmonyConsole.exe -i 10.0.0.1 -u email@example.com -p password -g

Protocol
--------
Check out the [protocol.md][protocol] at the pyharmony repo for the current
progress.

Thanks
------

With thanks to [petele] and [jterrace] for their work on pyharmony which 
pointed me in the right direction, first  found via [Pete's Blog][petelepage].
Also to the HarmonyHubControl[hhc] project on Sourceforge.

TODO
----

Same as the pyharmony guys - now I can talk to the Hub I need to understand
more about the protocol.

- Figure out how to detect when the session token expires so we can get a
new one.
- Figure out a good way of sending commands based on sync state.
- Is it possible to update device configuration?


[petelepage]:http://petelepage.com/blog/tag/harmony-hub-api/
[jterrace]:https://github.com/jterrace/pyharmony/
[petele]:https://github.com/petele/pyharmony
[protocol]:https://github.com/jterrace/pyharmony/blob/master/PROTOCOL.md
[x]:http://sourceforge.net/projects/harmonyhubcontrol/