# Overview

Ultimate Game Winner is a windows application that's the primary purpose of determining who the Ultimate Game Winner is by awarding points to players depending on their placements in logged board gameplays.
The user inputs various gameplays (name of the board game, the players and their placements). The program awards more points to those who place well in the longer and more complex board games.
The Weight, Game Length, Picture, and Genre of a board game are grabbed automatically through use of an API.
These point totals can be viewed through the Leaderboard, but the program can also act as a simple play tracking application.

# Development Environment

I created this code in Visual Studio using WPF, which utilizes XAML and C#.

# Functionalities/Features


- Filter System. Filter based off Game Weight, Game Length, Game Genre, Number of Players, and/or Groups.
- Custom Ranking System. Points are based of placement, game weight, and game length. User can add different weights to those 3 categories.
- Number of Wins Ranking System. Can changed the Leaderboard to be based off only number of first and second placements.
- View/Edit/Delete Indivdual recorded gameplays.
- View list of all gameplays individual people have participated in.
- Game Name Suggestions box. Auto-fills game names as you type to help you locate the specific game you were looking for.

# Acknowledgements

The database is powered by BoardGameGeek through use of their API.

# Useful Websites

- [YouTube: C# WPF Tutorial Playlist](https://www.youtube.com/playlist?list=PLih2KERbY1HHOOJ2C6FOrVXIwg4AZ-hk1)
- [YouTube: C# WPF and GUI - Pages and Navigation](https://www.youtube.com/watch?v=aBh0weP1bmo)
- [BoardGameGeek](https://boardgamegeek.com/)
- [ChatGPT](https://chatgpt.com/)

# Future Work

- Catch Up Mechanic. Players are awarded more points for beating higher ranked players than them and lose less points for losing to higher ranked players than them.
- Tags. Similar to groups but more flexible -- user picks what tags to add.
- Handles Ties.
- Handles Team vs Team games.
