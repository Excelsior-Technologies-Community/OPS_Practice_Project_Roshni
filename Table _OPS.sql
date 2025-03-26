CREATE TABLE [dbo].[tbl_Product] (
    [ProductId]    BIGINT         NOT NULL,
    [ProductName]  NVARCHAR (500) NOT NULL,
    [Description]  NVARCHAR (MAX) NULL,
    [Price]        DECIMAL(18, 2) NOT NULL,
    [Stock]        INT            NOT NULL,
    [ImageUrl]     NVARCHAR (MAX) NULL,
    [CategoryId]   BIGINT         NULL,
    [Flag]         NCHAR (1)      NULL,
    [CreateUser]   BIGINT         NULL,
    [CreateDate]   DATETIME       NULL,
    [UpdateUser]   BIGINT         NULL,
    [UpdateDate]   DATETIME       NULL,
    CONSTRAINT [PK_tbl_Product] PRIMARY KEY CLUSTERED ([ProductId] ASC)
);


CREATE TABLE [dbo].[tbl_Order] (
    [OrderId]      BIGINT         NOT NULL,
    [UserId]       BIGINT         NOT NULL,
    [OrderDate]    DATETIME       NOT NULL,
    [TotalAmount]  DECIMAL(18, 2) NOT NULL,
    [Status]       NVARCHAR (50)  NULL,
    [ShippingAddress] NVARCHAR (500) NULL,
    [Flag]         NCHAR (1)      NULL,
    [CreateUser]   BIGINT         NULL,
    [CreateDate]   DATETIME       NULL,
    [UpdateUser]   BIGINT         NULL,
    [UpdateDate]   DATETIME       NULL,
    CONSTRAINT [PK_tbl_Order] PRIMARY KEY CLUSTERED ([OrderId] ASC),
    CONSTRAINT [FK_Order_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[tbl_User_Mst] ([Id])
);


CREATE TABLE [dbo].[tbl_OrderDetails] (
    [OrderDetailsId] BIGINT         NOT NULL,
    [OrderId]        BIGINT         NOT NULL,
    [ProductId]      BIGINT         NOT NULL,
    [Quantity]       INT            NOT NULL,
    [UnitPrice]      DECIMAL(18, 2) NOT NULL,
    [TotalPrice]     AS ([Quantity] * [UnitPrice]) PERSISTED,
    [Flag]           NCHAR (1)      NULL,
    [CreateUser]     BIGINT         NULL,
    [CreateDate]     DATETIME       NULL,
    [UpdateUser]     BIGINT         NULL,
    [UpdateDate]     DATETIME       NULL,
    CONSTRAINT [PK_tbl_OrderDetails] PRIMARY KEY CLUSTERED ([OrderDetailsId] ASC),
    CONSTRAINT [FK_OrderDetails_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[tbl_Order] ([OrderId]),
    CONSTRAINT [FK_OrderDetails_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[tbl_Product] ([ProductId])
);


CREATE TABLE [dbo].[tbl_Review] (
    [ReviewId]     BIGINT         NOT NULL,
    [ProductId]    BIGINT         NOT NULL,
    [UserId]       BIGINT         NOT NULL,
    [Rating]       INT            CHECK ([Rating] BETWEEN 1 AND 5) NOT NULL,
    [ReviewText]   NVARCHAR (MAX) NULL,
    [ReviewDate]   DATETIME       DEFAULT GETDATE(),
    [Flag]         NCHAR (1)      NULL,
    [CreateUser]   BIGINT         NULL,
    [CreateDate]   DATETIME       NULL,
    [UpdateUser]   BIGINT         NULL,
    [UpdateDate]   DATETIME       NULL,
    CONSTRAINT [PK_tbl_Review] PRIMARY KEY CLUSTERED ([ReviewId] ASC),
    CONSTRAINT [FK_Review_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[tbl_Product] ([ProductId]),
    CONSTRAINT [FK_Review_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[tbl_User_Mst] ([Id])
);


INSERT INTO [dbo].[tbl_User_Type_Mst] ([UserType], [Flag], [CreateUser], [CreateDate], [UpdateUser], [UpdateDate])
VALUES 
    ('Admin', 'Y', 1, GETDATE(), NULL, NULL),
    ('User', 'Y', 1, GETDATE(), NULL, NULL),
    ('Guest', 'N', 1, GETDATE(), NULL, NULL);

