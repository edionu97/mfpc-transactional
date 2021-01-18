import { URI } from "./config";

const getProductsService = async () => {

  const response = await fetch(URI('products/all'));
  
  if (response.ok) {
    const result = await response.json();
    return result.products;
  } else {
    return [];
  }

};

export {
  getProductsService,
};
