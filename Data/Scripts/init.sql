-- User Table
CREATE TABLE User (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    AutoLogin INTEGER NULL DEFAULT 0
);

-- Party Table
CREATE TABLE Party (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);

-- Transaction Table
CREATE TABLE [Transaction] (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SenderId INTEGER NOT NULL,
    ReceiverId INTEGER NOT NULL,
    TransactionDate TEXT NOT NULL,
    Amount REAL NOT NULL CHECK (Amount > 0),
    Brokerage REAL NOT NULL CHECK (Brokerage >= 0),
    FOREIGN KEY (SenderId) REFERENCES Party(Id),
    FOREIGN KEY (ReceiverId) REFERENCES Party(Id),
    CHECK (SenderId != ReceiverId)
);

-- Default User
INSERT INTO User (Username, PasswordHash)
VALUES ('admin', '1OBAEzDswgKrhTwmJ2X6sA==.TJr8LrIAfXSvICc9x8s/BwElQd/l+OIdl8emv//PG/M=');