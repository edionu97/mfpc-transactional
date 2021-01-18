import React, { useState } from 'react';

import { Button, Checkbox, CircularProgress, Grid, InputAdornment, List, ListItem, ListItemIcon, ListItemText, makeStyles, TextField, Typography } from '@material-ui/core';
import { getAvailableProductsService } from '../services/getAvailableProductsService';
import { Search } from '@material-ui/icons';
import { getClientsService } from '../services/getClientsService';
import { addOrderService } from '../services/addOrder';

const useStyles = makeStyles({
  centered: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: 50,
  },
  placeButton: {
    borderColor: '#fff',
    color: '#fff',
    width: 200,
    marginTop: 20,
  },
  buttonGrid: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
  },
  list: {
    height: 300,
    overflowY: 'scroll',
    scrollbarWidth: 'none',
    '-ms-overflow-style': 'none',
  },
});

const PlaceOrder = () => {

  const classes = useStyles();

  const [availableProductsList, setAvailableProductsList] = useState([]);
  const [selectedProductsIds, setSelectedProductsIds] = useState([]);

  const [selectedProducts, setSelectedProducts] = useState({});

  // stored in two states, for refetching, because before deleting some record,
  // maybe you've edited the value of the text field, so it has to be stored the real CNP of the last fetch

  const [clientCNP, setClientCNP] = useState('');
  const [textInput, setTextInput] = useState('');
  const [loading, setLoading] = useState(false);

  const fetchAvailableProductsByCNP = async () => {
    const availableProducts = await getAvailableProductsService(textInput);
    setAvailableProductsList(
      availableProducts
    );
    
    // init object with products/quantities
    availableProducts.forEach(product => {
      selectedProducts[product.productId] = 0;  
    });
    setSelectedProducts(
      selectedProducts
    );

  };

  const getClientId = async () => {
    const registeredClients = await getClientsService();
    const clientWithGivenCNP = registeredClients.find(client => client.cnp === clientCNP);

    return clientWithGivenCNP.clientId;
  };

  const handleCheckProduct = (productId) => () => {
    const currentIndex = selectedProductsIds.indexOf(productId);
    const newChecked = [...selectedProductsIds];

    if (currentIndex === -1) {
      newChecked.push(productId);
      if (Number(selectedProducts[productId]) === 0) {
        setSelectedProducts({
          ...selectedProducts,
          [productId]: '1',
        });
      }
    } else {
      newChecked.splice(currentIndex, 1);
    }

    setSelectedProductsIds(newChecked);
  };

  const placeOrderHandle = async () => {
    const clientId = await getClientId();
    let payload = {
      clientId,
      orderedProducts: selectedProductsIds.map(productId => ({
        productId,
        itemsNo: Number(selectedProducts[productId]),
      })),
    };

    const result = await addOrderService(payload);
    if (result) {
      setTimeout(() => {
        fetchAvailableProductsByCNP(clientCNP);
      }, 1000);
    }
  };

  return (
    <Grid item container xs={12} className={classes.centered}>
      <Grid item xs={4} style={{ marginBottom: 10 }}>
        <TextField
          value={textInput}
          onChange={(event) => setTextInput(event.target.value)}
          variant='outlined'
          onKeyUp={(event) => {
            if (event.key === 'Enter') {
              setAvailableProductsList([]);
              setLoading(true);
              setClientCNP(textInput);
              fetchAvailableProductsByCNP(textInput);           
              setTimeout(() => {
                setLoading(false);
              }, 1500);
            }
          }}
          fullWidth
          label='Client CNP'
          InputProps={{
            endAdornment: (
              <InputAdornment>
                <Search />
              </InputAdornment>
            ),
          }}
        />
      </Grid>
      <Grid item container xs={10} style={{ marginBottom: 30, marginTop: 20 }} className={classes.centered}>
        {!loading ?
          availableProductsList.length !== 0 ?
            <>
              <Grid item xs={5} className={classes.list}>
                <List>
                  {
                    availableProductsList.map((product, index) => {
                      return (
                        <ListItem
                          key={index}
                          dense
                          button
                          onClick={handleCheckProduct(product.productId)}
                          style={{ backgroundColor: '#fff', marginBottom: 5 }}
                        >
                          <ListItemIcon>
                            <Checkbox
                              edge='start'
                              checked={selectedProductsIds.indexOf(product.productId) !== -1}
                              disableRipple
                              style={{ color: '#5f9ea0' }}
                            />
                          </ListItemIcon>
                          <ListItemText primary={`${product.productName} ${product.price}`} />
                          <TextField
                            variant='outlined'
                            InputProps={{
                              type: 'number',
                            }}
                            value={selectedProducts[product.productId]}
                            onChange={(event) => {
                              setSelectedProducts({
                                ...selectedProducts,
                                [product.productId]: event.target.value,
                              });
                            }}
                          />
                        </ListItem>
                      );
                    })
                  }
                </List>
              </Grid>
              <Grid item xs={8} className={classes.buttonGrid}>
                <Button 
                  fullWidth 
                  className={classes.placeButton}
                  variant='outlined'
                  disabled={selectedProductsIds.length === 0}
                  onClick={() => placeOrderHandle()}
                >
                  Place Order
                </Button>
              </Grid>
            </>
          : clientCNP !== '' && <Typography>No products available for this client</Typography>
        : <CircularProgress style={{ color: '#fff' }} />
      }
      </Grid>
    </Grid>
  );

};

export default PlaceOrder;
