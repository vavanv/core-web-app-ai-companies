-- SQL script to create the new tables for Companies, Chatbots, and LLMs
-- Run this in your SQLite database

-- Create Companies table
CREATE TABLE IF NOT EXISTS "Companies" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL
);

-- Create unique index on Company Name
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Companies_Name" ON "Companies" ("Name");

-- Create Chatbots table
CREATE TABLE IF NOT EXISTS "Chatbots" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "CompanyId" INTEGER NOT NULL,
    FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id") ON DELETE CASCADE
);

-- Create LLMs table
CREATE TABLE IF NOT EXISTS "LLMs" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Specialization" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "CompanyId" INTEGER NOT NULL,
    FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id") ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_Chatbots_CompanyId" ON "Chatbots" ("CompanyId");
CREATE INDEX IF NOT EXISTS "IX_LLMs_CompanyId" ON "LLMs" ("CompanyId");