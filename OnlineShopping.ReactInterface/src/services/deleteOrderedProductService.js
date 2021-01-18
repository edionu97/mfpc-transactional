import { URI } from "./config";

const deleteOrderedProductService = async (oldProduct) => {

  const payload = JSON.stringify(oldProduct);

  const response = await fetch(URI('orders/remove-product'), {
    method: 'DELETE',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
    },
    body: payload,
  });

  if (!response.ok) {
    const result = await response.json();
    return result.detail;
  }

  return response.ok;

};

export {
  deleteOrderedProductService,
};
