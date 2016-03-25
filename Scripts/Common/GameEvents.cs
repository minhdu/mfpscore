public static class GameEvents {

	public static class GameStateEvents {
		public static readonly string END_GAME = "END_GAME";
		public static readonly string MISSION_FAIL = "MISSION_FAIL";
		public static readonly string MISSION_COMPLETE = "MISSION_COMPLETE";
	}

	public static class GameplayEvents {
		public static readonly string DAMAGE = "DAMAGE_";
		public static readonly string ZOMBIE_DEAD = "ZOMBIE_DEAD";
	}
}
