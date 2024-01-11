# Flashcard Application
This C# application allows users to create and manage stacks of flashcards and conduct study sessions. The project utilizes SQL Server for database management, introduces the concept of Data Transfer Objects (DTOs), and ensures data integrity through foreign key relationships.

## Introduction
After gaining familiarity with C# in the initial projects, this project introduces the use of SQL Server for database management. The application involves the creation of stacks of flashcards, with each flashcard linked to a specific stack. Additionally, study sessions can be conducted and stored, providing users with a comprehensive learning experience.

## Requirement
- Two tables: Stacks and Flashcards, linked by a foreign key.
- Stacks should have unique names.
- Flashcards must be associated with a stack, and if a stack is deleted, its flashcards should also be deleted.
- Flashcard IDs in a stack should be consecutive without gaps.
- Study Sessions area to track user learning progress, storing date and score.
- Study sessions linked to stacks, and if a stack is deleted, its study sessions should be deleted.
- Use of DTOs to display flashcards to users without revealing the stack ID.
- The ability to view all study sessions without supporting update and delete operations.

## Getting Started
To get started with the project, follow these steps:

Clone the repository to your local machine.
Set up a SQL Server database and configure connection strings in the application.
Execute database scripts to create tables and relationships.
Run the application and start creating stacks, flashcards, and conducting study sessions.

## Functionality

Flashcards:
- Create stacks with unique names.
- Add, edit, and delete flashcards within a stack.
- Maintain consecutive IDs for flashcards in a stack.
- Ensure flashcards are deleted when their associated stack is deleted.

Study Sessions:
- Conduct study sessions on specific stacks.
- Store study sessions with date and score.
- View all study sessions, grouped by stack.

DTOs:
- Use DTOs to display flashcards without revealing stack IDs to users.

## Usage
- Run the application.
- Create stacks and add flashcards to them.
- Conduct study sessions and track learning progress.
- View study sessions to analyze performance.
