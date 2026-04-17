Video Demo Link: https://youtu.be/wDQjrD9pF2k

Project Name: Group16_iPERMITAPP Database Name: Group16_iPERMITDB (SQLite via DB helper class)

Project Overview: The iPERMIT Web Application is a full-stack MVC system designed to manage environmental permit applications. It supports two user roles:

RE (Registered Environmental users):
- Can register, log in, submit permit applications, and track status

EO (Environmental Officer):
- Can review applications, process payments, issue permits, update statuses, and view reports.
The system follows an MVC architecture using ASP.NET MVC and uses SQLite as the database, accessed through a built-in DB Helper class.

Technologies Used:
- ASP .NET MVC (VS)
- C#
- SQLite DB
- HTML / CSS / JavaScript
- DB helper class for DB access

Setup Instructions:
1. Download and unzip the provided project folder
2. Locate the publish file : /bin/app.publish
3. Double click to run the application
4. The application will automatically:
    - Initalize SQLite database if not created
    - Load required tables using DB Helper
    - Launch the web application in your default brower

If this does not work, please attempt to load the .sln (solution) file, build, and run that!
