import { URI } from "./config";

const getOrderedProductsByCNPService = async (CNP) => {

  const response = await fetch(URI(`orders/all/client?clientCNP=${CNP}`));
  const result = await response.json();
  
  if (response.ok) {
    
    return {
      success: true,
      data: result.messsage.map(order => {
        
        const splitByT = order.orderDate.split('T');
        const splitByColon = splitByT[1].split(':');
        
        return {
          orderId: order.orderId,
          productId: order.productId,
          clientId: order.clientId,
          orderDate: `${splitByT[0]} (${splitByColon[0]}:${splitByColon[1]})`,
          itemsNo: order.itemsNo,
          productName: order.orderedProducts[0].productName,
          price: order.orderedProducts[0].price,
          model: order.orderedProducts[0].model,
        };
      })
    };
  
  } else {

    // the error message
    return {
      success: false,
      msg: result.detail, 
    };
  }

};

export {
  getOrderedProductsByCNPService,
};
