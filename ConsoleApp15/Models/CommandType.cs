namespace ConsoleApp15.Models
{
    public enum CommandType
    {
        Help,
        Map,
        Version,
        Register,
        Login,
        Logout,
        WhoAmI,
        AddNewNote,
        ListNotes,
        DeleteNote,
        EditNote,
        Stats,
        StatsWatch,
        SecurityLogs,
        AdminUsers,

        AdminDeleteUser,
        AdminCreateAdmin,
        SetRole,
        CheckUpdate,
        ApplyUpdate,
        Interactive,
        Exit,
        Unknown
    }
}
