CREATE TABLE BrokerAccount (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
	PersonName TEXT NOT NULL,
    MobileNo TEXT,
    Address TEXT,
    BankName TEXT,
    AccountNumber TEXT,
    IFSCCode TEXT,
    PANNo TEXT
);

INSERT INTO BrokerAccount 
(Name, PersonName,MobileNo, Address, BankName, AccountNumber, IFSCCode, PANNo) 
VALUES 
('Chetan Brokers', 'CHETAN HARSUKHLAL BAKHAI', '98257 94142', 'Bhagvati palace, Kolki road, Upleta', 'Bank Of Baroda', '94870100011092', 'BARB0UPLETA', 'AYCPB5871E'),
('Keval Brokers', 'KEVAL CHETANBHAI BAKHAI', '97269 15827', 'Bhagvati palace, Kolki road, Upleta', 'State Bank of India', '42434360480', 'SBIN0005949', 'HOEPB5980R');



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
    Name TEXT NOT NULL UNIQUE,
    BrokerAccountId INTEGER,
    City TEXT,
    FOREIGN KEY (BrokerAccountId) REFERENCES BrokerAccount(Id)
);


-- Default User
INSERT INTO User (Username, PasswordHash)
VALUES ('admin', '1OBAEzDswgKrhTwmJ2X6sA==.TJr8LrIAfXSvICc9x8s/BwElQd/l+OIdl8emv//PG/M=');

-- Transaction Table
CREATE TABLE [Transaction] (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SenderId INTEGER NOT NULL,
    ReceiverId INTEGER NOT NULL,
    TransactionDate TEXT NOT NULL,
    Amount REAL NOT NULL,
    bagQuantity REAL NOT NULL,
    remarks TEXT,
    Brokerage REAL NOT NULL,
    FOREIGN KEY (SenderId) REFERENCES Party(Id),
    FOREIGN KEY (ReceiverId) REFERENCES Party(Id),
    CHECK (SenderId != ReceiverId)
);