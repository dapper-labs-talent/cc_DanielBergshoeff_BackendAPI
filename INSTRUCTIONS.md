STEP 1. Have postgreSQL installed on machine / install postgreSQL 
postgreSQL download link: https://www.postgresql.org/download/

STEP 2. Same for Visual Studio
Visual Studio download link: https://visualstudio.microsoft.com/downloads/

STEP 3. Open pgAdmin 4 (setup password as dapperdan or add your own password later in appsettings.json) 

STEP 4. Right mouse click on Databases -> Create -> Database..
Set name to dapperlabs and Owner to postgres and then press Save

STEP 5. Right click the dapperlabs database and press Query Tool

STEP 6. Press the Open File icon in the taskbar and open DapperProject/Database/Setup.sql

STEP 7. Once the Query has been loaded into your Query Editor, Execute the Query to create the database

STEP 8. Open DapperApp/DapperApp.sln with Visual Studio

STEP 9. Ensure that in appsettings.json the User Id and Password are set to match yours in postgres as well as the Port

STEP 10. Run program in Visual Studio

OPTIONAL
STEP 11. Use Postman to test API functionality
Base link is always http://localhost:{port}/api/dapperuser
Can be appended with /users, /signup or /login for CRUD functionality.

If you have any further questions, feel free to email me at dabergshoeff@gmail.com

