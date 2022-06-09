CREATE TABLE DapperUser(
	UserId serial,
	UserFirstName varchar(100),
	UserLastName varchar(100),
	UserMail varchar(100),
	UserPassword varchar(255)
);

INSERT INTO DapperUser(UserFirstName, UserLastName, UserMail, UserPassword) 
VALUES ('Daniel', 'Bergshoeff', 'dabergshoeff@gmail.com', '123456789');

SELECT * FROM DapperUser