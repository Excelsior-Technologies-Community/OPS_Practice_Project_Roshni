
INSERT INTO [dbo].[tbl_Country_Mst] 
( [Country], [Flag], [CreateUser], [CreateDate], [UpdateUser], [UpdateDate]) 
VALUES 
('India', 'Y', 101, GETDATE(), 101, GETDATE()),
('United States', 'Y', 102, GETDATE(), 102, GETDATE()),
('Canada', 'Y', 103, GETDATE(), 103, GETDATE()),
('Australia', 'N', 104, GETDATE(), 104, GETDATE());


INSERT INTO [dbo].[tbl_State_Mst] 
( [State], [CountryId], [CreateUser], [CreateDate], [UpdateUser], [UpdateDate]) 
VALUES
( 'Gujarat', 1, 101, GETDATE(), 101, GETDATE()),
('Maharashtra', 1, 101, GETDATE(), 101, GETDATE()),
('California', 2, 102, GETDATE(), 102, GETDATE()),
('Ontario', 3, 103, GETDATE(), 103, GETDATE()),
('New South Wales', 4, 104, GETDATE(), 104, GETDATE());



INSERT INTO [dbo].[tbl_City_Mst] 
([City], [StateId], [CreateUser], [CreateDate], [UpdateUser], [UpdateDate]) 
VALUES
('Ahmedabad', 1, 101, GETDATE(), 101, GETDATE()),
('Mumbai', 1, 101, GETDATE(), 101, GETDATE()),
('Los Angeles', 3, 102, GETDATE(), 102, GETDATE()),
('Toronto', 4, 103, GETDATE(), 103, GETDATE()),
('Sydney', 2, 104, GETDATE(), 104, GETDATE());

