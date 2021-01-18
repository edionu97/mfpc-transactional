import { URI } from "./config";

const addOrderService = async (newOrder) => {

  const payload = JSON.stringify(newOrder);

  const response = await fetch(URI('orders/add-new'), {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
    },
    body: payload,
  });

  return response.ok;

};

export {
  addOrderService,
};
