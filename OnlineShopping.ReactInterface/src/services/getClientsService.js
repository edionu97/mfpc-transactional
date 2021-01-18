import { URI } from "./config";

const getClientsService = async () => {

  const response = await fetch(URI('clients/all'));
  
  if (response.ok) {
    const result = await response.json();
    return result.clients;
  } else {
    return [];
  }

};

export {
  getClientsService,
};
