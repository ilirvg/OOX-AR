public static class DBManager {

    public static string username;
    public static string roomName;
    public static string cloudID;
    public static int score;
    public static int icon;
    public static int inGameScore = 0;
    public static int wins = 0;
    public static int loses = 0;
    public static int scale = 1;
    public static int planeType = 0;
    public static int nfcStatus = 0;
    public static bool hostClicked = false;
    public static bool joinClicked = false;
    public static bool inSettings = false;
    public static bool pcTesting = false;
    public static bool isGuest = false;
    public static bool inDistance = false;
    public static bool isSinglePlayer = false;
    public static bool isSkinChoosen = false;
    public static bool bothObjSpawn = false;
    public static bool bothObjSpawnDistance = false;
    public static int bothBoardsSpawn = 0;

    public static bool LoggedIn {
        get { return username != null; }
    }

    public static void LogOut() {
        username = null;
    }
}