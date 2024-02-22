# This is a demo created for Itsware by Juan Ordosgoite

Stack:
- C#
- React Native Windows
- SQLite

This project includes 2 folders:

- Server:
Socket server created with C#, to run just open a terminal, go to the server folder and enter the following command:

dotnet run

- Client:

Created using react native windows. Implementing react-native-tcp-sockets (the only library that claims support for multiple platforms, including Windows) to create the client connection to the socket server. It also includes react-native-sqlite-storage to handle the database.

To run the client, just open a terminal, go to the client folder and enter the following command:

npx react-native run-windows

Constrains:

It requires to create a SQlite database called devices.db and update the path indicated on the client file App.tsx (if necessary).