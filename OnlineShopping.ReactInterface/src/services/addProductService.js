import { URI } from "./config";

const addProductService = async (newProduct) => {

  const payload = JSON.stringify(newProduct);

  const response = await fetch(URI('products/add-new'), {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
    },
    body: payload,
  });

  if (!response.ok) {
    return 
  }

  return response.ok;

};

export {
  addProductService,
};
