using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InAppProductsList", menuName = "IAP/InAppProductsList", order = 1)]
public class InAppProductsList : ScriptableObject {

	[SerializeField] private List<InAppProduct> products;

	public List<InAppProduct> Products {
		get {
			return products;
		}
	}

	public InAppProduct GetProduct(string productId) {
		foreach (InAppProduct product in Products) {
			if (product.ProductId.Equals (productId)) {
				return product;
			}
		}

		return null;
	}

}