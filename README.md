# MySQL Repair Tool for XAMPP

![Preview](screenshots/app-preview01.png "App Preview")

A simple tool to repair corrupted MySQL databases on XAMPP for Windows. This is meant to fix a common issue `Error: MySQL shutdown unexpectedly` that appears when starting MySQL service on the XAMPP control panel.

This project is built using C# and .NET 8.0.

## Test Directory Builder
Script `TestDirectoryBuilder.ps1` creates a test directory structure with dummy MySQL database files to simulate the corrupted database scenario which allows testing of the functionality of the tool without risking actual data.👍