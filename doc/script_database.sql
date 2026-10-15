IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [migration_id] nvarchar(150) NOT NULL,
        [product_version] nvarchar(32) NOT NULL,
        CONSTRAINT [pk___ef_migrations_history] PRIMARY KEY ([migration_id])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [amenities] (
        [id] bigint NOT NULL IDENTITY,
        [name] nvarchar(100) NOT NULL,
        [icon] varchar(100) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_amenities] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [court_types] (
        [id] bigint NOT NULL IDENTITY,
        [name] nvarchar(100) NOT NULL,
        [icon] varchar(100) NULL,
        [description] nvarchar(255) NULL,
        [is_active] bit NOT NULL,
        [deleted_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_court_types] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [email_queue] (
        [id] bigint NOT NULL IDENTITY,
        [to_email] varchar(150) NOT NULL,
        [template] varchar(50) NOT NULL,
        [subject] nvarchar(200) NOT NULL,
        [body] nvarchar(max) NOT NULL,
        [status] varchar(30) NOT NULL,
        [retry_count] int NOT NULL,
        [send_after] datetime2(0) NULL,
        [sent_at] datetime2(0) NULL,
        [error] nvarchar(255) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_email_queue] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [job_runs] (
        [id] bigint NOT NULL IDENTITY,
        [job_name] varchar(50) NOT NULL,
        [started_at] datetime2(0) NOT NULL,
        [finished_at] datetime2(0) NULL,
        [status] varchar(30) NOT NULL,
        [affected_rows] int NOT NULL,
        [error] nvarchar(255) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_job_runs] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [roles] (
        [id] bigint NOT NULL IDENTITY,
        [code] varchar(30) NOT NULL,
        [name] nvarchar(100) NOT NULL,
        CONSTRAINT [pk_roles] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [audit_logs] (
        [id] bigint NOT NULL IDENTITY,
        [actor_user_id] bigint NULL,
        [actor_role] varchar(30) NOT NULL,
        [performed_by_name] nvarchar(100) NULL,
        [branch_id] bigint NULL,
        [action] varchar(80) NOT NULL,
        [entity_type] varchar(100) NOT NULL,
        [entity_id] bigint NOT NULL,
        [old_value] nvarchar(max) NULL,
        [new_value] nvarchar(max) NULL,
        [ip] varchar(50) NULL,
        [user_agent] varchar(255) NULL,
        [created_at] datetime2(0) NOT NULL,
        CONSTRAINT [pk_audit_logs] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [auth_tokens] (
        [id] bigint NOT NULL IDENTITY,
        [user_id] bigint NOT NULL,
        [type] varchar(30) NOT NULL,
        [token_hash] varchar(255) NOT NULL,
        [expires_at] datetime2(0) NOT NULL,
        [used_at] datetime2(0) NULL,
        [revoked_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_auth_tokens] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [booking_history] (
        [id] bigint NOT NULL IDENTITY,
        [booking_id] bigint NOT NULL,
        [action] varchar(30) NOT NULL,
        [old_value] nvarchar(max) NULL,
        [new_value] nvarchar(max) NULL,
        [price_diff] decimal(12,0) NOT NULL,
        [performed_by_user_id] bigint NULL,
        [performed_by_name] nvarchar(100) NULL,
        [created_at] datetime2(0) NOT NULL,
        CONSTRAINT [pk_booking_history] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [booking_reminders] (
        [booking_id] bigint NOT NULL,
        [type] varchar(30) NOT NULL,
        [sent_at] datetime2(0) NOT NULL,
        CONSTRAINT [pk_booking_reminders] PRIMARY KEY ([booking_id], [type])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [booking_services] (
        [id] bigint NOT NULL IDENTITY,
        [booking_id] bigint NOT NULL,
        [service_id] bigint NOT NULL,
        [name_snapshot] nvarchar(100) NOT NULL,
        [quantity] int NOT NULL,
        [unit_price_snapshot] decimal(12,0) NOT NULL,
        [amount] decimal(12,0) NOT NULL,
        [added_when] varchar(30) NOT NULL,
        [added_by_name] nvarchar(100) NULL,
        [added_at] datetime2(0) NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_booking_services] PRIMARY KEY ([id]),
        CONSTRAINT [CK_booking_services_qty] CHECK ([quantity] > 0 AND [unit_price_snapshot] >= 0)
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [booking_slots] (
        [id] bigint NOT NULL IDENTITY,
        [booking_id] bigint NOT NULL,
        [court_id] bigint NOT NULL,
        [slot_id] bigint NOT NULL,
        [slot_date] date NOT NULL,
        [start_at] datetime2(0) NOT NULL,
        [end_at] datetime2(0) NOT NULL,
        [slot_type_snapshot] varchar(30) NOT NULL,
        [day_type_snapshot] varchar(30) NOT NULL,
        [price_snapshot] decimal(12,0) NOT NULL,
        [status] varchar(30) NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_booking_slots] PRIMARY KEY ([id]),
        CONSTRAINT [CK_booking_slots_price] CHECK ([price_snapshot] >= 0),
        CONSTRAINT [CK_booking_slots_time] CHECK ([end_at] > [start_at])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [bookings] (
        [id] bigint NOT NULL IDENTITY,
        [code] varchar(20) NOT NULL,
        [customer_id] bigint NOT NULL,
        [branch_id] bigint NOT NULL,
        [source] varchar(30) NOT NULL,
        [status] varchar(30) NOT NULL,
        [payment_status] varchar(30) NOT NULL,
        [play_date] date NOT NULL,
        [start_at] datetime2(0) NOT NULL,
        [end_at] datetime2(0) NOT NULL,
        [slot_amount] decimal(12,0) NOT NULL,
        [service_amount] decimal(12,0) NOT NULL,
        [discount_amount] decimal(12,0) NOT NULL,
        [total_amount] decimal(12,0) NOT NULL,
        [deposit_percent_snapshot] int NOT NULL,
        [deposit_amount] decimal(12,0) NOT NULL,
        [paid_amount] decimal(12,0) NOT NULL,
        [refund_amount] decimal(12,0) NOT NULL,
        [cancel_policy_snapshot] nvarchar(max) NULL,
        [hold_expires_at] datetime2(0) NOT NULL,
        [recurring_id] bigint NULL,
        [voucher_id] bigint NULL,
        [qr_secret] varchar(64) NOT NULL,
        [checked_in_at] datetime2(0) NULL,
        [checkin_method] varchar(30) NULL,
        [checked_in_by_name] nvarchar(100) NULL,
        [completed_at] datetime2(0) NULL,
        [cancel_type] varchar(30) NULL,
        [cancel_reason] nvarchar(255) NULL,
        [cancelled_at] datetime2(0) NULL,
        [cancelled_by_user_id] bigint NULL,
        [cancelled_by_name] nvarchar(100) NULL,
        [created_by_user_id] bigint NOT NULL,
        [created_by_name] nvarchar(100) NULL,
        [note] nvarchar(255) NULL,
        [row_version] rowversion NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_bookings] PRIMARY KEY ([id]),
        CONSTRAINT [CK_bookings_cancel] CHECK ([status] NOT IN ('Cancelled','Expired','NoShow') OR ([cancel_type] IS NOT NULL AND [cancelled_at] IS NOT NULL)),
        CONSTRAINT [CK_bookings_cancel_reason] CHECK ([cancel_type] NOT IN ('Manager','Admin') OR LEN(LTRIM(RTRIM([cancel_reason]))) > 0),
        CONSTRAINT [CK_bookings_checkin] CHECK ([status] NOT IN ('CheckedIn','Completed') OR [checked_in_at] IS NOT NULL),
        CONSTRAINT [CK_bookings_deposit] CHECK ([deposit_amount] <= [total_amount] AND [refund_amount] <= [paid_amount]),
        CONSTRAINT [CK_bookings_hold] CHECK ([status] <> 'Holding' OR [hold_expires_at] IS NOT NULL),
        CONSTRAINT [CK_bookings_money] CHECK ([slot_amount] >= 0 AND [service_amount] >= 0 AND [discount_amount] >= 0 AND [deposit_amount] >= 0 AND [paid_amount] >= 0 AND [refund_amount] >= 0),
        CONSTRAINT [CK_bookings_percent] CHECK ([deposit_percent_snapshot] BETWEEN 0 AND 100),
        CONSTRAINT [CK_bookings_status] CHECK ([status] IN ('Holding','Deposited','CheckedIn','Completed','Cancelled','Expired','NoShow')),
        CONSTRAINT [CK_bookings_time] CHECK ([end_at] > [start_at]),
        CONSTRAINT [CK_bookings_total] CHECK ([total_amount] = [slot_amount] + [service_amount] - [discount_amount])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [invoices] (
        [id] bigint NOT NULL IDENTITY,
        [booking_id] bigint NOT NULL,
        [invoice_no] varchar(30) NOT NULL,
        [total_amount] decimal(12,0) NOT NULL,
        [issued_at] datetime2(0) NOT NULL,
        [issued_by_name] nvarchar(100) NULL,
        [pdf_path] varchar(255) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_invoices] PRIMARY KEY ([id]),
        CONSTRAINT [CK_invoices_total] CHECK ([total_amount] >= 0),
        CONSTRAINT [fk_invoices_bookings_booking_id] FOREIGN KEY ([booking_id]) REFERENCES [bookings] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [payments] (
        [id] bigint NOT NULL IDENTITY,
        [booking_id] bigint NOT NULL,
        [type] varchar(30) NOT NULL,
        [method] varchar(30) NOT NULL,
        [gateway] varchar(30) NULL,
        [amount] decimal(12,0) NOT NULL,
        [status] varchar(30) NOT NULL,
        [gateway_txn_id] varchar(100) NULL,
        [gateway_payload] nvarchar(max) NULL,
        [related_payment_id] bigint NULL,
        [received_by_name] nvarchar(100) NULL,
        [paid_at] datetime2(0) NULL,
        [note] nvarchar(255) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_payments] PRIMARY KEY ([id]),
        CONSTRAINT [CK_payments_amount] CHECK ([amount] >= 0),
        CONSTRAINT [fk_payments_bookings_booking_id] FOREIGN KEY ([booking_id]) REFERENCES [bookings] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_payments_payments_related_payment_id] FOREIGN KEY ([related_payment_id]) REFERENCES [payments] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [invoice_items] (
        [id] bigint NOT NULL IDENTITY,
        [invoice_id] bigint NOT NULL,
        [item_type] varchar(30) NOT NULL,
        [description] nvarchar(255) NOT NULL,
        [quantity] int NOT NULL,
        [unit_price] decimal(12,0) NOT NULL,
        [amount] decimal(12,0) NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_invoice_items] PRIMARY KEY ([id]),
        CONSTRAINT [fk_invoice_items_invoices_invoice_id] FOREIGN KEY ([invoice_id]) REFERENCES [invoices] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [branch_amenities] (
        [branch_id] bigint NOT NULL,
        [amenity_id] bigint NOT NULL,
        CONSTRAINT [pk_branch_amenities] PRIMARY KEY ([branch_id], [amenity_id]),
        CONSTRAINT [fk_branch_amenities_amenities_amenity_id] FOREIGN KEY ([amenity_id]) REFERENCES [amenities] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [branch_images] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NOT NULL,
        [url] varchar(255) NOT NULL,
        [sort_order] int NOT NULL,
        [is_cover] bit NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_branch_images] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [branch_policies] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NOT NULL,
        [deposit_percent] int NOT NULL,
        [allow_change_booking] bit NOT NULL,
        [change_before_hours] int NOT NULL,
        [max_hold_extend] int NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_branch_policies] PRIMARY KEY ([id]),
        CONSTRAINT [CK_branch_policies_deposit] CHECK ([deposit_percent] BETWEEN 0 AND 100)
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [branch_staff_names] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NOT NULL,
        [full_name] nvarchar(100) NOT NULL,
        [is_active] bit NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_branch_staff_names] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [branches] (
        [id] bigint NOT NULL IDENTITY,
        [owner_user_id] bigint NOT NULL,
        [name] nvarchar(150) NOT NULL,
        [address] nvarchar(255) NOT NULL,
        [district] nvarchar(100) NULL,
        [city] nvarchar(100) NULL,
        [latitude] decimal(10,7) NULL,
        [longitude] decimal(10,7) NULL,
        [phone] varchar(20) NULL,
        [description] nvarchar(max) NULL,
        [open_time] time NOT NULL,
        [close_time] time NOT NULL,
        [status] varchar(30) NOT NULL,
        [reject_reason] nvarchar(255) NULL,
        [lock_reason] nvarchar(255) NULL,
        [approved_by_user_id] bigint NULL,
        [approved_at] datetime2(0) NULL,
        [avg_rating] decimal(2,1) NOT NULL DEFAULT 0.0,
        [deleted_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_branches] PRIMARY KEY ([id]),
        CONSTRAINT [CK_branches_lat_long] CHECK (([latitude] IS NULL OR [latitude] BETWEEN -90 AND 90) AND ([longitude] IS NULL OR [longitude] BETWEEN -180 AND 180)),
        CONSTRAINT [CK_branches_open_close] CHECK ([open_time] < [close_time])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [cancel_policy_tiers] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NULL,
        [hours_before] int NOT NULL,
        [refund_percent] int NOT NULL,
        [sort_order] int NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_cancel_policy_tiers] PRIMARY KEY ([id]),
        CONSTRAINT [CK_cancel_policy_tiers_refund] CHECK ([refund_percent] BETWEEN 0 AND 100),
        CONSTRAINT [fk_cancel_policy_tiers_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [courts] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NOT NULL,
        [court_type_id] bigint NOT NULL,
        [name] nvarchar(100) NOT NULL,
        [description] nvarchar(max) NULL,
        [image_url] varchar(255) NULL,
        [capacity] int NOT NULL,
        [status] varchar(30) NOT NULL,
        [live_status] varchar(30) NOT NULL,
        [live_status_manual_until] datetime2(0) NULL,
        [deleted_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_courts] PRIMARY KEY ([id]),
        CONSTRAINT [CK_courts_capacity] CHECK ([capacity] > 0),
        CONSTRAINT [fk_courts_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_courts_court_types_court_type_id] FOREIGN KEY ([court_type_id]) REFERENCES [court_types] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [daily_closings] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NOT NULL,
        [closing_date] date NOT NULL,
        [cash_total] decimal(12,0) NOT NULL,
        [transfer_total] decimal(12,0) NOT NULL,
        [card_total] decimal(12,0) NOT NULL,
        [actual_cash] decimal(12,0) NOT NULL,
        [difference] decimal(12,0) NOT NULL,
        [closed_by_name] nvarchar(100) NULL,
        [note] nvarchar(255) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_daily_closings] PRIMARY KEY ([id]),
        CONSTRAINT [fk_daily_closings_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [holidays] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NULL,
        [holiday_date] date NOT NULL,
        [name] nvarchar(100) NOT NULL,
        [surcharge_percent] int NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_holidays] PRIMARY KEY ([id]),
        CONSTRAINT [CK_holidays_surcharge] CHECK ([surcharge_percent] >= 0),
        CONSTRAINT [fk_holidays_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [services] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NOT NULL,
        [name] nvarchar(100) NOT NULL,
        [unit] nvarchar(30) NOT NULL,
        [price] decimal(12,0) NOT NULL,
        [stock_qty] int NULL,
        [is_active] bit NOT NULL,
        [deleted_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_services] PRIMARY KEY ([id]),
        CONSTRAINT [CK_services_price] CHECK ([price] >= 0),
        CONSTRAINT [fk_services_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [time_slots] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NOT NULL,
        [slot_index] int NOT NULL,
        [start_time] time NOT NULL,
        [end_time] time NOT NULL,
        [slot_type] varchar(30) NOT NULL,
        [is_active] bit NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_time_slots] PRIMARY KEY ([id]),
        CONSTRAINT [fk_time_slots_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [users] (
        [id] bigint NOT NULL IDENTITY,
        [role_id] bigint NOT NULL,
        [branch_id] bigint NULL,
        [email] varchar(150) NULL,
        [phone] varchar(20) NULL,
        [password_hash] varchar(255) NULL,
        [full_name] nvarchar(150) NOT NULL,
        [avatar_url] varchar(255) NULL,
        [is_walk_in] bit NOT NULL,
        [status] varchar(30) NOT NULL,
        [lock_reason] nvarchar(255) NULL,
        [email_verified_at] datetime2(0) NULL,
        [no_show_count] int NOT NULL DEFAULT 0,
        [booking_restricted_until] datetime2(0) NULL,
        [require_full_prepay] bit NOT NULL,
        [last_login_at] datetime2(0) NULL,
        [deleted_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_users] PRIMARY KEY ([id]),
        CONSTRAINT [CK_users_branch_role] CHECK (([role_id] = 2 AND [branch_id] IS NOT NULL) OR ([role_id] <> 2 AND [branch_id] IS NULL)),
        CONSTRAINT [CK_users_no_show] CHECK ([no_show_count] >= 0),
        CONSTRAINT [CK_users_status] CHECK ([status] IN ('Active','Locked')),
        CONSTRAINT [fk_users_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_users_roles_role_id] FOREIGN KEY ([role_id]) REFERENCES [roles] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [vouchers] (
        [id] bigint NOT NULL IDENTITY,
        [code] varchar(30) NOT NULL,
        [branch_id] bigint NULL,
        [discount_type] varchar(30) NOT NULL,
        [value] decimal(12,0) NOT NULL,
        [max_discount] decimal(12,0) NULL,
        [min_order] decimal(12,0) NULL,
        [usage_limit] int NULL,
        [used_count] int NOT NULL,
        [valid_from] datetime2(0) NULL,
        [valid_to] datetime2(0) NULL,
        [is_active] bit NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_vouchers] PRIMARY KEY ([id]),
        CONSTRAINT [CK_vouchers_value] CHECK ([value] >= 0),
        CONSTRAINT [fk_vouchers_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [equipment] (
        [id] bigint NOT NULL IDENTITY,
        [court_id] bigint NOT NULL,
        [name] nvarchar(100) NOT NULL,
        [status] varchar(30) NOT NULL,
        [note] nvarchar(255) NULL,
        [deleted_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_equipment] PRIMARY KEY ([id]),
        CONSTRAINT [fk_equipment_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [price_rules] (
        [id] bigint NOT NULL IDENTITY,
        [branch_id] bigint NOT NULL,
        [court_type_id] bigint NULL,
        [court_id] bigint NULL,
        [day_type] varchar(30) NOT NULL,
        [slot_type] varchar(30) NOT NULL,
        [price] decimal(12,0) NOT NULL,
        [valid_from] date NOT NULL,
        [valid_to] date NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_price_rules] PRIMARY KEY ([id]),
        CONSTRAINT [CK_price_rules_dates] CHECK ([valid_to] IS NULL OR [valid_to] >= [valid_from]),
        CONSTRAINT [CK_price_rules_price] CHECK ([price] >= 0),
        CONSTRAINT [CK_price_rules_target] CHECK ([court_id] IS NOT NULL OR [court_type_id] IS NOT NULL),
        CONSTRAINT [fk_price_rules_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_price_rules_court_types_court_type_id] FOREIGN KEY ([court_type_id]) REFERENCES [court_types] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_price_rules_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [slot_blocks] (
        [id] bigint NOT NULL IDENTITY,
        [court_id] bigint NOT NULL,
        [start_date] date NOT NULL,
        [end_date] date NOT NULL,
        [start_slot_id] bigint NULL,
        [end_slot_id] bigint NULL,
        [block_type] varchar(30) NOT NULL,
        [reason] nvarchar(255) NULL,
        [performed_by_name] nvarchar(100) NULL,
        [cancelled_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_slot_blocks] PRIMARY KEY ([id]),
        CONSTRAINT [CK_slot_blocks_dates] CHECK ([end_date] >= [start_date]),
        CONSTRAINT [CK_slot_blocks_slots] CHECK (([start_slot_id] IS NULL AND [end_slot_id] IS NULL) OR ([start_slot_id] IS NOT NULL AND [end_slot_id] IS NOT NULL)),
        CONSTRAINT [fk_slot_blocks_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_slot_blocks_time_slots_end_slot_id] FOREIGN KEY ([end_slot_id]) REFERENCES [time_slots] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_slot_blocks_time_slots_start_slot_id] FOREIGN KEY ([start_slot_id]) REFERENCES [time_slots] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [broadcasts] (
        [id] bigint NOT NULL IDENTITY,
        [sent_by_user_id] bigint NOT NULL,
        [title] nvarchar(255) NOT NULL,
        [content] nvarchar(max) NOT NULL,
        [target] varchar(30) NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_broadcasts] PRIMARY KEY ([id]),
        CONSTRAINT [fk_broadcasts_users_sent_by_user_id] FOREIGN KEY ([sent_by_user_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [chat_threads] (
        [id] bigint NOT NULL IDENTITY,
        [type] varchar(30) NOT NULL,
        [customer_id] bigint NULL,
        [branch_id] bigint NULL,
        [status] varchar(30) NOT NULL,
        [last_message_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_chat_threads] PRIMARY KEY ([id]),
        CONSTRAINT [fk_chat_threads_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE SET NULL,
        CONSTRAINT [fk_chat_threads_users_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [users] ([id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [complaints] (
        [id] bigint NOT NULL IDENTITY,
        [code] varchar(20) NOT NULL,
        [customer_id] bigint NOT NULL,
        [branch_id] bigint NOT NULL,
        [booking_id] bigint NULL,
        [title] nvarchar(200) NOT NULL,
        [content] nvarchar(max) NOT NULL,
        [status] varchar(30) NOT NULL,
        [escalated_at] datetime2(0) NULL,
        [resolution] nvarchar(max) NULL,
        [resolved_by_user_id] bigint NULL,
        [resolved_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_complaints] PRIMARY KEY ([id]),
        CONSTRAINT [fk_complaints_bookings_booking_id] FOREIGN KEY ([booking_id]) REFERENCES [bookings] ([id]) ON DELETE SET NULL,
        CONSTRAINT [fk_complaints_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_complaints_users_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_complaints_users_resolved_by_user_id] FOREIGN KEY ([resolved_by_user_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [customer_violations] (
        [id] bigint NOT NULL IDENTITY,
        [user_id] bigint NOT NULL,
        [booking_id] bigint NOT NULL,
        [type] varchar(30) NOT NULL,
        [action_taken] varchar(30) NOT NULL,
        [resolved_by_user_id] bigint NULL,
        [resolved_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_customer_violations] PRIMARY KEY ([id]),
        CONSTRAINT [fk_customer_violations_bookings_booking_id] FOREIGN KEY ([booking_id]) REFERENCES [bookings] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_customer_violations_users_resolved_by_user_id] FOREIGN KEY ([resolved_by_user_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_customer_violations_users_user_id] FOREIGN KEY ([user_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [favorites] (
        [id] bigint NOT NULL IDENTITY,
        [customer_id] bigint NOT NULL,
        [branch_id] bigint NOT NULL,
        [court_id] bigint NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_favorites] PRIMARY KEY ([id]),
        CONSTRAINT [fk_favorites_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_favorites_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_favorites_users_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [users] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [notifications] (
        [id] bigint NOT NULL IDENTITY,
        [user_id] bigint NOT NULL,
        [type] varchar(50) NOT NULL,
        [title] nvarchar(255) NOT NULL,
        [body] nvarchar(max) NOT NULL,
        [link] varchar(255) NULL,
        [is_read] bit NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_notifications] PRIMARY KEY ([id]),
        CONSTRAINT [fk_notifications_users_user_id] FOREIGN KEY ([user_id]) REFERENCES [users] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [recurring_bookings] (
        [id] bigint NOT NULL IDENTITY,
        [customer_id] bigint NOT NULL,
        [branch_id] bigint NOT NULL,
        [court_id] bigint NOT NULL,
        [weekday] tinyint NOT NULL,
        [slot_id] bigint NOT NULL,
        [start_date] date NOT NULL,
        [end_date] date NULL,
        [weeks_ahead] int NOT NULL,
        [generated_until] date NOT NULL,
        [discount_percent] int NOT NULL,
        [status] varchar(30) NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_recurring_bookings] PRIMARY KEY ([id]),
        CONSTRAINT [CK_recurring_bookings_weekday] CHECK ([weekday] BETWEEN 1 AND 7),
        CONSTRAINT [fk_recurring_bookings_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_recurring_bookings_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_recurring_bookings_time_slots_slot_id] FOREIGN KEY ([slot_id]) REFERENCES [time_slots] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_recurring_bookings_users_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [reviews] (
        [id] bigint NOT NULL IDENTITY,
        [booking_id] bigint NOT NULL,
        [customer_id] bigint NOT NULL,
        [branch_id] bigint NOT NULL,
        [court_id] bigint NOT NULL,
        [rating] tinyint NOT NULL,
        [comment] nvarchar(max) NULL,
        [deleted_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_reviews] PRIMARY KEY ([id]),
        CONSTRAINT [CK_reviews_rating] CHECK ([rating] BETWEEN 1 AND 5),
        CONSTRAINT [fk_reviews_bookings_booking_id] FOREIGN KEY ([booking_id]) REFERENCES [bookings] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_reviews_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_reviews_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_reviews_users_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [system_settings] (
        [key] varchar(100) NOT NULL,
        [value] nvarchar(max) NOT NULL,
        [is_encrypted] bit NOT NULL,
        [description] nvarchar(255) NULL,
        [updated_by_user_id] bigint NULL,
        [updated_at] datetime2(0) NOT NULL,
        CONSTRAINT [pk_system_settings] PRIMARY KEY ([key]),
        CONSTRAINT [fk_system_settings_users_updated_by_user_id] FOREIGN KEY ([updated_by_user_id]) REFERENCES [users] ([id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [waitlist] (
        [id] bigint NOT NULL IDENTITY,
        [customer_id] bigint NOT NULL,
        [court_id] bigint NOT NULL,
        [slot_date] date NOT NULL,
        [slot_id] bigint NOT NULL,
        [status] varchar(30) NOT NULL,
        [notified_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_waitlist] PRIMARY KEY ([id]),
        CONSTRAINT [fk_waitlist_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_waitlist_time_slots_slot_id] FOREIGN KEY ([slot_id]) REFERENCES [time_slots] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_waitlist_users_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [incidents] (
        [id] bigint NOT NULL IDENTITY,
        [court_id] bigint NOT NULL,
        [equipment_id] bigint NULL,
        [description] nvarchar(max) NOT NULL,
        [severity] varchar(30) NOT NULL,
        [image_url] varchar(255) NULL,
        [reported_by_name] nvarchar(100) NULL,
        [status] varchar(30) NOT NULL,
        [set_maintenance] bit NOT NULL,
        [resolved_note] nvarchar(255) NULL,
        [resolved_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_incidents] PRIMARY KEY ([id]),
        CONSTRAINT [fk_incidents_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_incidents_equipment_equipment_id] FOREIGN KEY ([equipment_id]) REFERENCES [equipment] ([id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [slot_locks] (
        [id] bigint NOT NULL IDENTITY,
        [court_id] bigint NOT NULL,
        [slot_date] date NOT NULL,
        [slot_id] bigint NOT NULL,
        [source_type] varchar(30) NOT NULL,
        [booking_slot_id] bigint NULL,
        [block_id] bigint NULL,
        [locked_at] datetime2(0) NOT NULL,
        [row_version] rowversion NOT NULL,
        CONSTRAINT [pk_slot_locks] PRIMARY KEY ([id]),
        CONSTRAINT [CK_slot_locks_source] CHECK (([source_type] = 'Booking' AND [booking_slot_id] IS NOT NULL AND [block_id] IS NULL) OR ([source_type] = 'Block' AND [block_id] IS NOT NULL AND [booking_slot_id] IS NULL)),
        CONSTRAINT [fk_slot_locks_booking_slots_booking_slot_id] FOREIGN KEY ([booking_slot_id]) REFERENCES [booking_slots] ([id]) ON DELETE CASCADE,
        CONSTRAINT [fk_slot_locks_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [fk_slot_locks_slot_blocks_block_id] FOREIGN KEY ([block_id]) REFERENCES [slot_blocks] ([id]) ON DELETE CASCADE,
        CONSTRAINT [fk_slot_locks_time_slots_slot_id] FOREIGN KEY ([slot_id]) REFERENCES [time_slots] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [chat_messages] (
        [id] bigint NOT NULL IDENTITY,
        [thread_id] bigint NOT NULL,
        [sender_type] varchar(30) NOT NULL,
        [sender_id] bigint NULL,
        [content] nvarchar(max) NOT NULL,
        [meta] nvarchar(max) NULL,
        [read_at] datetime2(0) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_chat_messages] PRIMARY KEY ([id]),
        CONSTRAINT [fk_chat_messages_chat_threads_thread_id] FOREIGN KEY ([thread_id]) REFERENCES [chat_threads] ([id]) ON DELETE CASCADE,
        CONSTRAINT [fk_chat_messages_users_sender_id] FOREIGN KEY ([sender_id]) REFERENCES [users] ([id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [complaint_messages] (
        [id] bigint NOT NULL IDENTITY,
        [complaint_id] bigint NOT NULL,
        [sender_id] bigint NOT NULL,
        [sender_role] varchar(30) NOT NULL,
        [content] nvarchar(max) NOT NULL,
        [attachment_urls] nvarchar(max) NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_complaint_messages] PRIMARY KEY ([id]),
        CONSTRAINT [fk_complaint_messages_complaints_complaint_id] FOREIGN KEY ([complaint_id]) REFERENCES [complaints] ([id]) ON DELETE CASCADE,
        CONSTRAINT [fk_complaint_messages_users_sender_id] FOREIGN KEY ([sender_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [recurring_skips] (
        [id] bigint NOT NULL IDENTITY,
        [recurring_id] bigint NOT NULL,
        [skip_date] date NOT NULL,
        [reason] varchar(30) NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_recurring_skips] PRIMARY KEY ([id]),
        CONSTRAINT [fk_recurring_skips_recurring_bookings_recurring_id] FOREIGN KEY ([recurring_id]) REFERENCES [recurring_bookings] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE TABLE [review_replies] (
        [id] bigint NOT NULL IDENTITY,
        [review_id] bigint NOT NULL,
        [user_id] bigint NOT NULL,
        [content] nvarchar(max) NOT NULL,
        [created_at] datetime2(0) NOT NULL,
        [updated_at] datetime2(0) NULL,
        CONSTRAINT [pk_review_replies] PRIMARY KEY ([id]),
        CONSTRAINT [fk_review_replies_reviews_review_id] FOREIGN KEY ([review_id]) REFERENCES [reviews] ([id]) ON DELETE CASCADE,
        CONSTRAINT [fk_review_replies_users_user_id] FOREIGN KEY ([user_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_audit_logs_actor_user_id] ON [audit_logs] ([actor_user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_audit_logs_branch_id_created_at] ON [audit_logs] ([branch_id], [created_at]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_audit_logs_created_at] ON [audit_logs] ([created_at]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_auth_tokens_user_id] ON [auth_tokens] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_booking_history_booking_id] ON [booking_history] ([booking_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_booking_history_performed_by_user_id] ON [booking_history] ([performed_by_user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_booking_services_booking_id] ON [booking_services] ([booking_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_booking_services_service_id] ON [booking_services] ([service_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_booking_slots_booking_id_court_id_slot_date_slot_id] ON [booking_slots] ([booking_id], [court_id], [slot_date], [slot_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_booking_slots_court_id_slot_date] ON [booking_slots] ([court_id], [slot_date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_booking_slots_slot_id] ON [booking_slots] ([slot_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_bookings_branch_id_play_date] ON [bookings] ([branch_id], [play_date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_bookings_cancelled_by_user_id] ON [bookings] ([cancelled_by_user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_bookings_code] ON [bookings] ([code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_bookings_created_by_user_id] ON [bookings] ([created_by_user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_bookings_customer_id] ON [bookings] ([customer_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_bookings_recurring_id] ON [bookings] ([recurring_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_bookings_status_hold_expires_at] ON [bookings] ([status], [hold_expires_at]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_bookings_status_start_at] ON [bookings] ([status], [start_at]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_bookings_voucher_id] ON [bookings] ([voucher_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_branch_amenities_amenity_id] ON [branch_amenities] ([amenity_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_branch_images_branch_id] ON [branch_images] ([branch_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_branch_policies_branch_id] ON [branch_policies] ([branch_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_branch_staff_names_branch_id_is_active] ON [branch_staff_names] ([branch_id], [is_active]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_branches_approved_by_user_id] ON [branches] ([approved_by_user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_branches_owner_user_id_name] ON [branches] ([owner_user_id], [name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_broadcasts_sent_by_user_id] ON [broadcasts] ([sent_by_user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_cancel_policy_tiers_branch_id_hours_before] ON [cancel_policy_tiers] ([branch_id], [hours_before]) WHERE [branch_id] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_chat_messages_sender_id] ON [chat_messages] ([sender_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_chat_messages_thread_id_created_at] ON [chat_messages] ([thread_id], [created_at]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_chat_threads_branch_id] ON [chat_threads] ([branch_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_chat_threads_customer_id] ON [chat_threads] ([customer_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_complaint_messages_complaint_id] ON [complaint_messages] ([complaint_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_complaint_messages_sender_id] ON [complaint_messages] ([sender_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_complaints_booking_id] ON [complaints] ([booking_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_complaints_branch_id] ON [complaints] ([branch_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_complaints_code] ON [complaints] ([code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_complaints_customer_id] ON [complaints] ([customer_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_complaints_resolved_by_user_id] ON [complaints] ([resolved_by_user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_court_types_name] ON [court_types] ([name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_courts_branch_id_name] ON [courts] ([branch_id], [name]) WHERE [deleted_at] IS NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_courts_court_type_id] ON [courts] ([court_type_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_customer_violations_booking_id] ON [customer_violations] ([booking_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_customer_violations_resolved_by_user_id] ON [customer_violations] ([resolved_by_user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_customer_violations_user_id] ON [customer_violations] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_daily_closings_branch_id_closing_date] ON [daily_closings] ([branch_id], [closing_date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_equipment_court_id] ON [equipment] ([court_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_favorites_branch_id] ON [favorites] ([branch_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_favorites_court_id] ON [favorites] ([court_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_favorites_customer_id_branch_id_court_id] ON [favorites] ([customer_id], [branch_id], [court_id]) WHERE [court_id] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_holidays_branch_id_holiday_date] ON [holidays] ([branch_id], [holiday_date]) WHERE [branch_id] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_incidents_court_id] ON [incidents] ([court_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_incidents_equipment_id] ON [incidents] ([equipment_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_invoice_items_invoice_id] ON [invoice_items] ([invoice_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_invoices_booking_id] ON [invoices] ([booking_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_invoices_invoice_no] ON [invoices] ([invoice_no]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_notifications_user_id_is_read] ON [notifications] ([user_id], [is_read]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_payments_booking_id] ON [payments] ([booking_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_payments_gateway_txn_id] ON [payments] ([gateway_txn_id]) WHERE [gateway_txn_id] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_payments_related_payment_id] ON [payments] ([related_payment_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_price_rules_branch_id_court_type_id_court_id_day_type_slot_type_valid_from] ON [price_rules] ([branch_id], [court_type_id], [court_id], [day_type], [slot_type], [valid_from]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_price_rules_court_id] ON [price_rules] ([court_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_price_rules_court_type_id] ON [price_rules] ([court_type_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_recurring_bookings_branch_id] ON [recurring_bookings] ([branch_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_recurring_bookings_court_id] ON [recurring_bookings] ([court_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_recurring_bookings_customer_id_court_id_weekday_slot_id] ON [recurring_bookings] ([customer_id], [court_id], [weekday], [slot_id]) WHERE [status] = ''Active''');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_recurring_bookings_slot_id] ON [recurring_bookings] ([slot_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_recurring_skips_recurring_id_skip_date] ON [recurring_skips] ([recurring_id], [skip_date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_review_replies_review_id] ON [review_replies] ([review_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_review_replies_user_id] ON [review_replies] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_reviews_booking_id] ON [reviews] ([booking_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_reviews_branch_id] ON [reviews] ([branch_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_reviews_court_id] ON [reviews] ([court_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_reviews_customer_id] ON [reviews] ([customer_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_roles_code] ON [roles] ([code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_services_branch_id_name] ON [services] ([branch_id], [name]) WHERE [deleted_at] IS NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_slot_blocks_court_id] ON [slot_blocks] ([court_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_slot_blocks_end_slot_id] ON [slot_blocks] ([end_slot_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_slot_blocks_start_slot_id] ON [slot_blocks] ([start_slot_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_slot_locks_block_id] ON [slot_locks] ([block_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_slot_locks_booking_slot_id] ON [slot_locks] ([booking_slot_id]) WHERE [booking_slot_id] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_slot_locks_court_id_slot_date_slot_id] ON [slot_locks] ([court_id], [slot_date], [slot_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_slot_locks_slot_id] ON [slot_locks] ([slot_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_system_settings_updated_by_user_id] ON [system_settings] ([updated_by_user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_time_slots_branch_id_slot_index] ON [time_slots] ([branch_id], [slot_index]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_time_slots_branch_id_start_time] ON [time_slots] ([branch_id], [start_time]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_users_branch_id] ON [users] ([branch_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_users_email] ON [users] ([email]) WHERE [email] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_users_phone] ON [users] ([phone]) WHERE [phone] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_users_role_id] ON [users] ([role_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_vouchers_branch_id] ON [vouchers] ([branch_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [ix_vouchers_code] ON [vouchers] ([code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_waitlist_court_id] ON [waitlist] ([court_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [ix_waitlist_customer_id_court_id_slot_date_slot_id] ON [waitlist] ([customer_id], [court_id], [slot_date], [slot_id]) WHERE [status] = ''Waiting''');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    CREATE INDEX [ix_waitlist_slot_id] ON [waitlist] ([slot_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [audit_logs] ADD CONSTRAINT [fk_audit_logs_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [audit_logs] ADD CONSTRAINT [fk_audit_logs_users_actor_user_id] FOREIGN KEY ([actor_user_id]) REFERENCES [users] ([id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [auth_tokens] ADD CONSTRAINT [fk_auth_tokens_users_user_id] FOREIGN KEY ([user_id]) REFERENCES [users] ([id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [booking_history] ADD CONSTRAINT [fk_booking_history_bookings_booking_id] FOREIGN KEY ([booking_id]) REFERENCES [bookings] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [booking_history] ADD CONSTRAINT [fk_booking_history_users_performed_by_user_id] FOREIGN KEY ([performed_by_user_id]) REFERENCES [users] ([id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [booking_reminders] ADD CONSTRAINT [fk_booking_reminders_bookings_booking_id] FOREIGN KEY ([booking_id]) REFERENCES [bookings] ([id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [booking_services] ADD CONSTRAINT [fk_booking_services_bookings_booking_id] FOREIGN KEY ([booking_id]) REFERENCES [bookings] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [booking_services] ADD CONSTRAINT [fk_booking_services_services_service_id] FOREIGN KEY ([service_id]) REFERENCES [services] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [booking_slots] ADD CONSTRAINT [fk_booking_slots_bookings_booking_id] FOREIGN KEY ([booking_id]) REFERENCES [bookings] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [booking_slots] ADD CONSTRAINT [fk_booking_slots_courts_court_id] FOREIGN KEY ([court_id]) REFERENCES [courts] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [booking_slots] ADD CONSTRAINT [fk_booking_slots_time_slots_slot_id] FOREIGN KEY ([slot_id]) REFERENCES [time_slots] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [bookings] ADD CONSTRAINT [fk_bookings_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [bookings] ADD CONSTRAINT [fk_bookings_recurring_bookings_recurring_id] FOREIGN KEY ([recurring_id]) REFERENCES [recurring_bookings] ([id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [bookings] ADD CONSTRAINT [fk_bookings_users_cancelled_by_user_id] FOREIGN KEY ([cancelled_by_user_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [bookings] ADD CONSTRAINT [fk_bookings_users_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [bookings] ADD CONSTRAINT [fk_bookings_users_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [bookings] ADD CONSTRAINT [fk_bookings_vouchers_voucher_id] FOREIGN KEY ([voucher_id]) REFERENCES [vouchers] ([id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [branch_amenities] ADD CONSTRAINT [fk_branch_amenities_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [branch_images] ADD CONSTRAINT [fk_branch_images_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [branch_policies] ADD CONSTRAINT [fk_branch_policies_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [branch_staff_names] ADD CONSTRAINT [fk_branch_staff_names_branches_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [branches] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [branches] ADD CONSTRAINT [fk_branches_users_approved_by_user_id] FOREIGN KEY ([approved_by_user_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    ALTER TABLE [branches] ADD CONSTRAINT [fk_branches_users_owner_user_id] FOREIGN KEY ([owner_user_id]) REFERENCES [users] ([id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [migration_id] = N'20261015090804_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([migration_id], [product_version])
    VALUES (N'20261015090804_InitialCreate', N'8.0.11');
END;
GO

COMMIT;
GO

