using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardboardRealityClient {
	
	/// <summary>
	/// Specifies the behaviour for a URI handler.
	/// </summary>
	public interface IUriHandler {

		/// <summary>
		/// Handles the specified URI and returns a value indicating whether any action was taken.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		bool HandleUri(Uri uri);
	}
}
