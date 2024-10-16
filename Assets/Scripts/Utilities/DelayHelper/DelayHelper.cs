using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class DelayHelper
{
	public static Task Delay(float seconds)
	{
		var tcs = new TaskCompletionSource<bool>();

		ScreenManager.Instance.StartCoroutine(DelayCoroutine(seconds, tcs));

		return tcs.Task;
	}

	private static IEnumerator DelayCoroutine(float seconds, TaskCompletionSource<bool> tcs)
	{
		yield return new WaitForSeconds(seconds);
		tcs.SetResult(true);
	}
}