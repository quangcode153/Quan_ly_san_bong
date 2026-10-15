namespace BTLtruc_quan.Shared.Enums;

public enum ComplaintStatus
{
    New = 1,
    BranchReplied = 2,
    Escalated = 3,
    Resolved = 4,
    Rejected = 5
}

public enum ComplaintSenderRole
{
    Customer = 1,
    Branch = 2,
    Admin = 3
}

public enum ChatThreadType
{
    CustomerBranch = 1,
    AdminBranch = 2,
    CustomerAi = 3
}

public enum ChatSenderType
{
    User = 1,
    Ai = 2,
    System = 3
}
