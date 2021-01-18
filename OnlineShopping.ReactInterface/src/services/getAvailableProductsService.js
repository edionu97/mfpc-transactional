import { URI } from "./config";

const getAvailableProductsService = async (CNP) => {

  const response = await fetch(URI(`orders/available-products-for-client?clientCnp=${CNP}`));
  
  if (response.ok) {
    const result = await response.json();
    return result.products;
  } else {
    return [];
  }

};

export {
  getAvailableProductsService,
};
