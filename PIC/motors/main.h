#include <18F46K22.h>
//#DEVICE ADC=16
//#DEVICE HIGH_INTS=TRUE
#device WRITE_EEPROM=ASYNC 

//#FUSES HS                       //High speed osc with HW enabled 4X PLL
#FUSES PLLEN
#FUSES PRIMARY_ON
#FUSES NOMCLR   
#FUSES WDT4096                   //Watch Dog Timer uses 1:128 Postscale = 2048*4ms
#FUSES PUT                      //Power up timer enabled
#FUSES NOFCMEN                  //Fail-safe clock monitor enabled
#FUSES NOIESO                   //Internal External Switch Over mode enabled
#FUSES NOBROWNOUT               //No brownout reset
#FUSES CCP2C1
#FUSES NOSTVREN                   //Stack full/underflow will not cause reset
#FUSES NOPBADEN                 //PORTB pins are configured as digital I/O on RESET
//#FUSES NOLPT1OSC                //Timer1 configured for higher power operation
#FUSES TIMER3C0              
#FUSES NOLVP                    //No low voltage prgming, B3(PIC16) or B5(PIC18) used for I/O
#FUSES NOXINST                  //Extended set extension and Indexed Addressing mode disabled (Legacy mode)
#FUSES NODEBUG                  //No Debug mode for ICD
#FUSES NOPROTECT                //Code not protected from reading
#FUSES NOCPB                    //No Boot Block code protection
#FUSES NOCPD                    //No EE protection
#FUSES NOWRT                    //Program memory not write protected
#FUSES NOWRTC                   //configuration not registers write protected
#FUSES NOWRTB                   //Boot block not write protected
#FUSES NOWRTD                   //Data EEPROM not write protected
#FUSES NOEBTR                   //Memory not protected from table reads
#FUSES NOEBTRB                  //Boot block not protected from table reads

#USE delay(clock=64000000, crystal=16000000)
//from controller
//#USE SPI (SLAVE, SPI2, BITS=8, STREAM=SPI_2, MSB_FIRST)
//ad9833 and 74hc165s
#USE SPI (MASTER, SPI1, BAUD = 16000000, MODE=2, BITS=16, STREAM=SPI_1, MSB_FIRST)



#use i2c(SLAVE, I2C2, force_hw, address=0x12) //RA
//#use i2c(SLAVE, I2C2, force_hw, address=0x26) //DEC
const int1 isRA=1;


//AD9833 commands
#define FSYNC PIN_C6
#define DISABLED PIN_C2
#define DIRECTION PIN_C1
//#define SPICLK PIN_C3
//#define SPIOUT PIN_C5

#DEFINE RESET 0b0010000101101000
#DEFINE UNSET 0b0010000001101000

//7hc165s
#define LOAD PIN_D5


//
#define STEPIN PIN_B0
#define GUIDE1 PIN_B1
#define GUIDE2 PIN_B2

#DEFINE WORM0 PIN_B4
#DEFINE WORM1 PIN_B5
#DEFINE WORM2 PIN_B6
#DEFINE WORM3 PIN_B7
#DEFINE WORM4 PIN_E3
#DEFINE WORM5 PIN_E0
#DEFINE WORM6 PIN_E1
#DEFINE WORM7 PIN_E2


