#include <18F46K22.h>
#device ADC=16
#device WRITE_EEPROM=ASYNC 

//#FUSES HSH                       //High speed osc with HW enabled 4X PLL
#FUSES PLLEN
#FUSES PRIMARY_ON
#FUSES NOMCLR   
#FUSES WDT4096                   //Watch Dog Timer uses 1:128 Postscale = 2048*4ms
#FUSES PUT                      //Power up timer enabled
#FUSES NOFCMEN                  //Fail-safe clock monitor enabled
#FUSES NOIESO                   //Internal External Switch Over mode enabled
#FUSES NOBROWNOUT               //No brownout reset
//#FUSES CCP2C1
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
#USE rs232(baud=115200,UART1, parity=N, bits=8, stream=handset, ERRORS)
#USE rs232(baud=115200,UART2, parity=N, bits=8, stream=pc, ERRORS)
#USE i2c(Master, I2C2, fast=200000, force_hw, stream=lcd)
#USE i2c(Master, I2C1, fast=400000, force_hw, stream=motor)
//#USE SPI (MASTER, SPI1, BAUD = 800000, MODE=2, BITS=8, STREAM=SPI_1, MSB_FIRST) //SPI is software now

//#DEFINE SS1 PIN_C1
#DEFINE RA_ADD  0x12
#DEFINE DEC_ADD  0x26
//#define SLAVE1_WRITE  0x12
//#define SLAVE1_READ  0x13 

#DEFINE FOCUSA PIN_B4
#DEFINE FOCUSB PIN_B5

#DEFINE FOCUSPOS 1
#DEFINE AMMETER1 1
#DEFINE AMMETER2 2
