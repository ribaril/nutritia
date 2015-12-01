using System.Collections.Generic;

namespace Nutritia.UI.Views
{
	internal interface ISuiviPlatService
	{
		int RetrieveSome(RetrieveSuiviPlatArgs retrieveSuiviPlatArgs);
		void Insert(List<Plat> listePlatsRetires, Membre membreCourant);
		void Update(List<Plat> listePlatsNonAdmissibles, Membre membreCourant);
	}
}