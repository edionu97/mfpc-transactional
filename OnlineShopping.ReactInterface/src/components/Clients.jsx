import React, { forwardRef, useMemo, useEffect, useState } from 'react';

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
import { CircularProgress, Grid, makeStyles, Typography } from '@material-ui/core';
import { getClientsService } from '../services/getClientsService';
import { addClientService } from '../services/addClientService';

import PersonAdd from '@material-ui/icons/PersonAdd';

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
    marginBottom: 30,
  },
});

const Clients = () => {

  const classes = useStyles();
  const [clientsList, setClientsList] = useState([]);
  const [loading, setLoading] = useState(true);

  const columns = useMemo(() => [
    { 
      title: 'ID', 
      field: 'clientId',
      editable: 'never',
      cellStyle: {
        width: '5%',
        textAlign: 'left',
        paddingLeft: 50,
      },
      headerStyle: {
        width: '5%',
        textAlign: 'left',
        paddingLeft: 50,
      },
    },
    { 
      title: 'First Name', 
      field: 'firstName',
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
      title: 'Last Name', 
      field: 'lastName',
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
      title: 'CNP', 
      field: 'cnp',
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
      title: 'Phone Number', 
      field: 'phoneNumber',
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
      title: 'Email', 
      field: 'email',
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
  ], []);

  useEffect(() => {
    fetchClients();
  }, []);

  const fetchClients = async () => {
    const clients = await getClientsService();
    setClientsList(clients);
    setTimeout(() => {
      setLoading(false);
    }, 2000);
  };

  const addClient = (client) =>
  new Promise(async (resolve, reject) => {
    
    if (
      !isNaN(Number(client.phoneNumber)) &&
      !clientsList.map(client => client.cnp).includes(client.cnp) &&
      client.cnp.length === 13 &&
      client.email.length > 10 &&
      client.email.includes('@') &&
      client.email.includes('.') &&
      client.firstName.length > 0 &&
      client.lastName.length > 0
    ) {
      const addResult = await addClientService(client);
      if (addResult) {
        await fetchClients();
        return resolve();
      } else {
        return reject();
      }
    } else {
      return reject();
    }

  })

  return (
    <Grid item container xs={10} className={classes.centered}>
      { !loading ?
        <MaterialTable
          options={{
            search: true,
            actionsColumnIndex: 6,
          }}
          icons={{
            ...tableIcons,
            Add: props => <PersonAdd style={{ color: '#5f9ea0' }} {...props}/>,
          }}
          title={
            <Typography style={{ fontSize: 20, color: '#5f9ea0' }}>Clients</Typography>
          }
          columns={columns}
          data={clientsList}
          editable={{
            onRowAdd: newData => addClient(newData), 
          }}
        />
        : <CircularProgress style={{ color: '#fff' }}/>
      }
    </Grid>
  );

};

export default Clients;
