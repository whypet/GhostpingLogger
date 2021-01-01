# GhostpingLogger - the bot that logs ghostpings
discord bot that logs ghostpings.



# how do work
it detects message edit and message delete events that previously had a mentionned user

# does this have a database?
technically yes but that's because it only uses JSON
to get the bot's token and command prefix.

# what does this thing use
it uses DSharpPlus (latest version, compiled dll reference) for the bot to work.

# can i use it
yes of course just selfhost it or something it only works on one server because of no
database so you might need to host multiple.

# can i fork it
yes and if you do i'd recommend implementing an SQL database for it.

# programming language?
c#, using .net core
