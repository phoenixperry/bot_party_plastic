using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMarkovMusic : AbstractMarkovMusic {
	public TestMarkovMusic() : base() {
	}
	public TestMarkovMusic(string file_data) : base(file_data) {
	}
	public override float getNextNoteLength (int second_last, int last)
	{
		return UnityEngine.Random.Range (2, 4) * 2;
	}
}
