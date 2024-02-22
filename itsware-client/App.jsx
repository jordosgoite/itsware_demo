import React, {useState, useEffect} from 'react';
import {Button, FlatList, Text, View} from 'react-native';
import TcpSocket from 'react-native-tcp-socket';
import SQLiteStorage from 'react-native-sqlite-storage';

//Reminder: All the db management should be handled by the server
//Reminder: Create Stylesheets to handle the styles
const db = SQLiteStorage.openDatabase({
  name: 'devices.db',
  location: 'C:sqlite',
});

db.transaction(tx => {
  tx.executeSql('CREATE TABLE IF NOT EXISTS devices (id TEXT PRIMARY KEY)');
  tx.executeSql(
    'CREATE TABLE IF NOT EXISTS audit_log (deviceId TEXT, checkoutTime DATETIME)',
    [],
  );
});
const App = () => {
  const [deviceList, setDeviceList] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const getDeviceList = async () => {
    try {
      setLoading(true);
      setError(null);
      const client = await TcpSocket.createConnection({port: 40001});
      const response = await client.readData();
      client.destroy();

      const devices = response.slice(response.indexOf(',') + 1, -1).split(',');
      setDeviceList(devices);

      devices.forEach(deviceId => {
        db.transaction(tx => {
          tx.executeSql('INSERT OR IGNORE INTO devices (id) VALUES (?)', [
            deviceId,
          ]);
        });
      });
    } catch (err) {
      console.error('Error getting device list:', err);
      setError('Failed to retrieve device list. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const checkoutDevice = async deviceId => {
    try {
      let checkoutTime = null;
      const query = 'SELECT datetime("now") AS checkoutTime';
      await db.transaction(tx => {
        tx.executeSql(query, [], (tx, results) => {
          checkoutTime = results.rows.item(0).checkoutTime;
        });
      });

      const client = await TcpSocket.createConnection({port: 40001});
      const message = `CheckoutDevice,${deviceId},${checkoutTime}`;
      await client.writeData(message);
      client.destroy();
      await db.transaction(tx => {
        tx.executeSql(
          'INSERT INTO audit_log (deviceId, checkoutTime) VALUES (?, ?)',
          [deviceId, checkoutTime],
        );
      });

      console.log(`Device ${deviceId} checked out successfully.`);
    } catch (err) {
      console.error('Error checking out device:', err);
    }
  };

  useEffect(() => {
    getDeviceList();
  }, []);

  return (
    <View style={{padding: 20}}>
      <Text>Device List:</Text>
      {loading ? (
        <Text>Loading devices...</Text>
      ) : error ? (
        <Text style={{color: 'red'}}>{error}</Text>
      ) : (
        <View>
          {deviceList.length > 0 ? (
            <FlatList
              data={deviceList}
              renderItem={({item}) => (
                <View key={item} style={{borderBottomWidth: 1, padding: 5}}>
                  <Text>{item}</Text>
                  <Button
                    title="Checkout"
                    onPress={() => checkoutDevice(item)}
                    disabled={loading}
                  />
                </View>
              )}
            />
          ) : (
            <Text>No devices found.</Text>
          )}
          <Button title="Get Device List" onPress={getDeviceList} />
        </View>
      )}
    </View>
  );
};

export default App;
