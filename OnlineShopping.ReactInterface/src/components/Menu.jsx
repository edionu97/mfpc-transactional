import React, { useContext } from 'react';
import { Button, Grid, makeStyles } from '@material-ui/core';

import { Context } from '../context/index';

const useStyles = makeStyles({
  centered: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: '10%',
    border: '1px solid #fff',
    borderRadius: 5,
    padding: 10,
    backgroundColor: '#fff',
  },
  menuButton: {
    borderColor: '#5f9ea0',
    color: '#000',
    backgroundColor: '#fff',
    fontFamily: 'Verdana',
  },
  menuButtonContainer: {
    margin: 10,
  }
});

const Menu = () => {

  const classes = useStyles();
  const appContext = useContext(Context);

  const {
    setView,
  } = appContext;

  return (
    <Grid item container xs={4} className={classes.centered}>
      <Grid item xs={12} className={classes.menuButtonContainer}>
        <Button 
          variant='outlined'
          className={classes.menuButton}
          fullWidth
          onClick={() => setView('clients')}
        >
          Clients
        </Button>
      </Grid>
      <Grid item xs={12} className={classes.menuButtonContainer}>
        <Button 
          variant='outlined'
          className={classes.menuButton}
          fullWidth
          onClick={() => setView('products')}
        >
          Products
        </Button>
      </Grid>
      <Grid item xs={12} className={classes.menuButtonContainer}>
        <Button 
          variant='outlined'
          className={classes.menuButton}
          fullWidth
          onClick={() => setView('orders')}
        >
          Ordered Products
        </Button>
      </Grid>
      <Grid item xs={12} className={classes.menuButtonContainer}>
        <Button 
          variant='outlined'
          className={classes.menuButton}
          fullWidth
          onClick={() => setView('place-order')}
        >
          Place order
        </Button>
      </Grid>
    </Grid>
  );

};

export default Menu;
