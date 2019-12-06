using System.Web.Services;

namespace VersionChecker
{
	/// <summary>
	/// Summary description for VersionChecker
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class VersionChecker : WebService
	{
		/// <summary>
		/// Compares two version strings 
		/// Assumes both version strings are comprised of dot-delimited integers
		/// </summary>
		/// <param name="version1">The first version string to compare</param>
		/// <param name="version2">The second version string to compare</param>
		/// <returns>"before" if the first version string came first
		/// "after" if it came after, and "equal" if they were the same version.
		/// Returns "error" if input assumptions are violated.
		/// </returns>
		[WebMethod]
		public string CompareVersions(string version1, string version2)
		{
			// Test input assumptions
			if (!tryParseVersionStringsAsInts(version1.Split('.'), out int[] version1SubVersions)) return "error";
			if (!tryParseVersionStringsAsInts(version2.Split('.'), out int[] version2SubVersions)) return "error";
			if (version1SubVersions.Length == 0 || version2SubVersions.Length == 0) return "error";

			int result = 0;

			// If the first array has more sub-versions than the second, flip them around first
			if (version1SubVersions.Length > version2SubVersions.Length)
			{
				result = -beforeOrAfter(version2SubVersions, version1SubVersions);
			}
			else
			{
				result = beforeOrAfter(version1SubVersions, version2SubVersions);
			}

			if (result == -1)
			{
				return "before";
			}
			else if (result == 0)
			{
				return "equal";
			}
			else
			{
				return "after";
			}
		}

		/// <summary>
		/// Attempts to parse all strings in a string array as ints
		/// </summary>
		/// <param name="subVersionStrings">Input list of strings to interpret</param>
		/// <param name="subVersionsNumbers">Output list of ints</param>
		/// <returns>True and an array of ints if successful. False and a partially-interpretted array otherwise</returns>
		private bool tryParseVersionStringsAsInts(string[] subVersionStrings, out int[] subVersionsNumbers)
		{
			subVersionsNumbers = new int[subVersionStrings.Length];

			// Parse each string in the input array, but exit early if any of them are unparseable.
			for (int i = 0; i < subVersionStrings.Length; i++)
			{
				if (!int.TryParse(subVersionStrings[i], out subVersionsNumbers[i])) return false;
			}

			return true;
		}

		/// <summary>
		/// Identifies if the first array of version numbers came before or after the second
		/// The first array MUST have fewer than or the same number of elements as the second.
		/// </summary>
		/// <param name="version1Subversions">First array of version ints</param>
		/// <param name="version2Subversions">Second array of version ints</param>
		/// <returns>-1 if the first array of ints came before, 1 if after, and 0 if they are the same</returns>
		private int beforeOrAfter(int[] version1Subversions, int[] version2Subversions)
		{
			int i = 0;

			for (; i < version1Subversions.Length; i++)
			{
				if (version1Subversions[i] > version2Subversions[i]) return 1;
				else if (version1Subversions[i] < version2Subversions[i]) return -1;
			}

			// Special case where the second array is the same version, but was appended with a bunch of zeros.
			for (; i < version2Subversions.Length; i++)
			{
				if (version2Subversions[i] != 0) return -1;
			}

			return 0;
		}
	}
}
