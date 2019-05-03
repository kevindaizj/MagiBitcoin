﻿DROP TABLE BASE_ADDRESS;
CREATE TABLE BASE_ADDRESS(
ID VARCHAR(50) PRIMARY KEY, 
ADDRESS VARCHAR(100) NOT NULL,
EXTPUBKEY_WIF NVARCHAR(200) NULL,
WALLET_ID VARCHAR(50) NULL,
NETWORK INTEGER NOT NULL DEFAULT(1), -- 1: Mainnet 2: Testnet(regtest)
KEY_PATH NVARCHAR(500) NULL,
PARENT_KEY_PATH NVARCHAR(500) NULL,
PATH_INDEX INTEGER NOT NULL DEFAULT(0),
ADDRESS_TYPE INTEGER NOT NULL, -- 0: ROOT 1: Company 2: Customer
CUSTOMER_ID INTEGER NULL,
ADDRESS_CATEGORY INTEGER NOT NULL, -- 0: Default 1: PAYER 2: RECEIVER
NAME NVARCHAR(500) NULL, 
BALANCE REAL NOT NULL DEFAULT(0),
ACCOUNT NVARCHAR(200) NULL,
DESCRIPTION NVARCHAR(500) NULL,
CREATE_DATE DATETIME NOT NULL);


DROP TABLE BASE_WALLET;
CREATE TABLE BASE_WALLET(
ID VARCHAR(50) PRIMARY KEY, 
WALLET_NAME NVARCHAR(100) NOT NULL,
PASSWORD NVARCHAR(200) NOT NULL,
MNEMONIC_WORDS TEXT NULL, -- just for test, you should keep mnemonic words pirvately
DESCRIPTION NVARCHAR(500) NULL,
IS_ACTIVE BOOLEAN NOT NULL,
CREATE_DATE DATETIME NOT NULL);