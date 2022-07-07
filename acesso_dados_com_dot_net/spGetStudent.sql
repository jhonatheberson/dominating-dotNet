CREATE OR ALTER PROCEDURE [spGetStudent] 
    @StudentId UNIQUEIDENTIFIER
AS
    SELECT * FROM [Student] WHERE [Id] = @StudentId