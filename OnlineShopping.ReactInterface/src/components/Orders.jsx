import React, { forwardRef, useMemo, useState } from 'react';

import MaterialTable from 'material-table';

import AddBox from '@material-ui/icons/AddBox';
import ArrowUpward from '@material-ui/icons/ArrowUpward';
import Check from '@material-ui/icons/Check';
import ChevronLeft from '@material-ui/icons/ChevronLeft';
import ChevronRight from '@material-ui/icons/ChevronRight';
import Clear from '@material-ui/icons/Clear';
import DeleteOutline from '@material-ui/icons/DeleteOutline';
import Edit from '@material-ui/icons/Edit';
import FilterList from '@material-ui/icons/FilterList';
import FirstPage from '@material-ui/icons/FirstPage';
import LastPage from '@material-ui/icons/LastPage';
import Remove from '@material-ui/icons/Remove';
import SaveAlt from '@material-ui/icons/SaveAlt';
import Search from '@material-ui/icons/Search';
import ViewColumn from '@material-ui/icons/ViewColumn';
import { CircularProgress, Grid, InputAdornment, makeStyles, TextField, Typography } from '@material-ui/core';
import { getOrderedProductsByCNPService } from '../services/getOrderedProductsByCNPService';
import { Delete } from '@material-ui/icons';
import { deleteOrderedProductService } from '../services/deleteOrderedProductService';

const tableIcons = {
  Add: forwardRef((props, ref) => <AddBox {...props} ref={ref} />),
  Check: forwardRef((props, ref) => <Check {...props} ref={ref} />),
  Clear: forwardRef((props, ref) => <Clear {...props} ref={ref} />),
  Delete: forwardRef((props, ref) => <DeleteOutline {...props} ref={ref} />),
  DetailPanel: forwardRef((props, ref) => <ChevronRight {...props} ref={ref} />),
  Edit: forwardRef((props, ref) => <Edit {...props} ref={ref} />),
  Export: forwardRef((props, ref) => <SaveAlt {...props} ref={ref} />),
  Filter: forwardRef((props, ref) => <FilterList {...props} ref={ref} />),
  FirstPage: forwardRef((props, ref) => <FirstPage {...props} ref={ref} />),
  LastPage: forwardRef((props, ref) => <LastPage {...props} ref={ref} />),
  NextPage: forwardRef((props, ref) => <ChevronRight {...props} ref={ref} />),
  PreviousPage: forwardRef((props, ref) => <ChevronLeft {...props} ref={ref} />),
  ResetSearch: forwardRef((props, ref) => <Clear {...props} ref={ref} />),
  Search: forwardRef((props, ref) => <Search {...props} ref={ref} />),
  SortArrow: forwardRef((props, ref) => <ArrowUpward {...props} ref={ref} />),
  ThirdStateCheck: forwardRef((props, ref) => <Remove {...props} ref={ref} />),
  ViewColumn: forwardRef((props, ref) => <ViewColumn {...props} ref={ref} />)
};

const useStyles = makeStyles({
  centered: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: 50,
  },
});

const Orders = () => {

  const classes = useStyles();

  const [orderedProductsList, setOrderedProductsList] = useState([]);
  
  // stored in two states, for refetching, because before deleting some record,
  // maybe you've edited the value of the text field, so it has to be stored the real CNP of the last fetch

  const [clientCNP, setClientCNP] = useState('');
  const [textInput, setTextInput] = useState('');
  const [loading, setLoading] = useState(false);

  const columns = useMemo(() => [
    { 
      title: 'OrderID', 
      field: 'orderId',
      cellStyle: {
        width: '5%',
        textAlign: 'left',
        paddingLeft: 30,
      },
      headerStyle: {
        width: '5%',
        textAlign: 'left',
        paddingLeft: 30,
      },
    },
    { 
      title: 'Order Date', 
      field: 'orderDate',
      cellStyle: {
        width: '20%',
        textAlign: 'left',
        paddingLeft: 30,
      },
      headerStyle: {
        width: '20%',
        textAlign: 'left',
        paddingLeft: 30,
      },
    },
    { 
      title: 'ProductID', 
      field: 'productId',
      cellStyle: {
        width: '5%',
        textAlign: 'left',
        paddingLeft: 30,
      },
      headerStyle: {
        width: '5%',
        textAlign: 'left',
        paddingLeft: 30,
      },
    },
    { 
      title: 'Product Name', 
      field: 'productName',
      cellStyle: {
        width: '20%',
        textAlign: 'left',
        paddingLeft: 10,
      },
      headerStyle: {
        width: '20%',
        textAlign: 'left',
        paddingLeft: 10,
      },
    },
    { 
      title: 'Price', 
      field: 'price',
      cellStyle: {
        width: '15%',
        textAlign: 'left',
        paddingLeft: 10,
      },
      headerStyle: {
        width: '15%',
        textAlign: 'left',
        paddingLeft: 10,
      },
    },
    {
      title: 'Model', 
      field: 'model',
      cellStyle: {
        width: '15%',
        textAlign: 'left',
        paddingLeft: 10,
      },
      headerStyle: {
        width: '15%',
        textAlign: 'left',
        paddingLeft: 10,
      },
    },
    {
      title: 'Quantity', 
      field: 'itemsNo',
      cellStyle: {
        width: '10%',
        textAlign: 'left',
        paddingLeft: 10,
      },
      headerStyle: {
        width: '10%',
        textAlign: 'left',
        paddingLeft: 10,
      },
    },
  ], []);

  const fetchOrderedProductsByCNP = async () => {
    const result = await getOrderedProductsByCNPService(textInput);

    if (result.success) {
      setOrderedProductsList(result.data);
    } else {
      // when we delete the last element, it raises an error
      if (result.msg.includes('any')) {
        setOrderedProductsList([]);
      }
    }
  };

  const deleteOrderedProduct = (oldData) =>
  new Promise( async (resolve, reject) => {

    const deleteResult = await deleteOrderedProductService({
      // not displaying clientId on each ordered product but it's stored in the fetched list
      clientId: orderedProductsList[0].clientId,
      productId: oldData.productId,
    });

    if (deleteResult) {
      await fetchOrderedProductsByCNP(clientCNP);
      return resolve();
    } else {
      return reject();
    }

  });

  return (
    <Grid item container xs={12} className={classes.centered}>
      <Grid item xs={4} style={{ marginBottom: 20 }}>
        <TextField
          value={textInput}
          onChange={(event) => setTextInput(event.target.value)}
          variant='outlined'
          onKeyUp={(event) => {
            if (event.key === 'Enter') {
              setOrderedProductsList([]);
              setLoading(true);
              setClientCNP(textInput);
              fetchOrderedProductsByCNP(textInput);           
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
        <Grid item xs={10} style={{ marginBottom: 30 }} className={classes.centered}>
      { !loading ?
          <MaterialTable
            options={{
              actionsColumnIndex: 7,
              search: true,
            }}
            icons={{
              ...tableIcons,
              Delete: props => <Delete style={{ color: 'red' }} {...props} />,
            }}
            title={
              <Typography style={{ fontSize: 20, color: '#5f9ea0' }}>Ordered Products</Typography>
            }
            editable={{
              onRowDelete: oldData => deleteOrderedProduct(oldData),
            }}
            columns={columns}
            data={orderedProductsList}
          />
          : <CircularProgress style={{ color: '#fff' }} />
      }
        </Grid>
    </Grid>
  );

};

export default Orders;
