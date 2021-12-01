# ReelCheck I/O
## LabelD (automatic)
### Inputs
| I/O Input type/port | Meaning      |
|---------------------|--------------|
| IO 1                | Enable       |
| IOEXT 1             | StickingDone |
| IOEXT 2             | Status Reset |
| IOEXT 3             | HardReset    |
### Outputs
| I/O Output type/port | Meaning            |
|----------------------|--------------------|
| IO 1 (Green)         | Ready              |
| IO 2 (Red)           | LabelPrinted       |
| IOEXT 1              | AllStationFinished |
| IOEXT 2              | WorkpieceOK        |
| IOEXT 3              | WorkpieceNOK       |
| IOEXT 4              | EMPTY              |

## LabelE (half automatic)
### Inputs
| I/O Input type/port | Meaning           |
|---------------------|-------------------|
| IOEXT 1             | Enable            |
| IOEXT 2             | StickingDone      |
| IOEXT 3             | IdCameraEnable    |
| IOEXT 4             | CheckCameraEnable |
### Outputs
| I/O Output type/port | Meaning            |
|----------------------|--------------------|
| IOEXT 1              | Ready              |
| IOEXT 2              | LabelPrinted       |
| IO Green             | WorkpieceOK        |
| IO Red               | WorkpieceNOK       |
| IOEXT 3              | IdCameraReady      |
