﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg.Cams
{
    internal static class AndorStatusMessages
    {
        public static readonly Dictionary<uint, string> Messages = new Dictionary<uint, string>()
        {
            {20001, "DRV_ERROR_CODES"}, {20065, "DRV_DRIVER_ERRORS"},
            {20002, "DRV_SUCCESS"}, {20066, "DRV_P1INVALID"},
            {20003, "DRV_VXDNOTINSTALLED"}, {20067, "DRV_P2INVALID"},
            {20004, "DRV_ERROR_SCAN"}, {20068, "DRV_P3INVALID"},
            {20005, "DRV_ERROR_CHECK_SUM"}, {20069, "DRV_P4INVALID"},
            {20006, "DRV_ERROR_FILELOAD"}, {20070, "DRV_INIERROR"},
            {20007, "DRV_UNKNOWN_FUNCTION"}, {20071, "DRV_COFERROR"},
            {20008, "DRV_ERROR_VXD_INIT"}, {20072, "DRV_ACQUIRING"},
            {20009, "DRV_ERROR_ADDRESS"}, {20073, "DRV_IDLE"},
            {20010, "DRV_ERROR_PAGELOCK"}, {20074, "DRV_TEMPCYCLE"},
            {20011, "DRV_ERROR_PAGE_UNLOCK"}, {20075, "DRV_NOT_INITIALIZED"},
            {20012, "DRV_ERROR_BOARDTEST"}, {20076, "DRV_P5INVALID"},
            {20013, "DRV_ERROR_ACK"}, {20077, "DRV_P6INVALID"},
            {20014, "DRV_ERROR_UP_FIFO"}, {20078, "DRV_INVALID_MODE"},
            {20015, "DRV_ERROR_PATTERN"}, {20079, "DRV_INVALID_FILTER"},
            {20017, "DRV_ACQUISITION_ERRORS"}, {20080, "DRV_I2CERRORS"},
            {20018, "DRV_ACQ_BUFFER"}, {20081, "DRV_DRV_I2CDEVNOTFOUND"},
            {20019, "DRV_ACQ_DOWNFIFO_FULL"}, {20082, "DRV_I2CTIMEOUT"},
            {20020, "DRV_PROC_UNKNOWN_INSTRUCTION"}, {20083, "DRV_P7INVALID"},
            {20021, "DRV_ILLEGAL_OP_CODE"}, {20089, "DRV_USBERROR"},
            {20022, "DRV_KINETIC_TIME_NOT_MET"}, {20090, "DRV_IOCERROR"},
            {20023, "DRV_ACCUM_TIME_NOT_MET"}, {20091, "DRV_VRMVERSIONERROR"},
            {20024, "DRV_NO_NEW_DATA"}, {20093, "DRV_USB_INTERRUPT_ENDPOINT_ERROR"},
            {20025, "PCI_DMA_FAIL"}, {20094, "DRV_RANDOM_TRACK_ERROR"},
            {20026, "DRV_SPOOLERROR"}, {20095, "DRV_INVALID_TRIGGER_MODE"},
            {20027, "DRV_SPOOLSETUPERROR"}, {20096, "DRV_LOAD_FIRMWARE_ERROR"},
            {20029, "SATURATED"}, {20097, "DRV_DIVIDE_BY_ZERO_ERROR"},
            {20033, "DRV_TEMPERATURE_CODES"}, {20098, "DRV_INVALID_RINGEXPOSURES"},
            {20034, "DRV_TEMPERATURE_OFF"}, {20099, "DRV_BINNING_ERROR"},
            {20035, "DRV_TEMP_NOT_STABILIZED"}, {20990, "DRV_ERROR_NOCAMERA"},
            {20036, "DRV_TEMPERATURE_STABILIZED"}, {20991, "DRV_NOT_SUPPORTED"},
            {20037, "DRV_TEMPERATURE_NOT_REACHED"}, {20992, "DRV_NOT_AVAILABLE"},
            {20038, "DRV_TEMPERATURE_OUT_RANGE"}, {20115, "DRV_ERROR_MAP"},
            {20039, "DRV_TEMPERATURE_NOT_SUPPORTED"}, {20116, "DRV_ERROR_UNMAP"},
            {20040, "DRV_TEMPERATURE_DRIFT"}, {20117, "DRV_ERROR_MDL"},
            {20049, "DRV_GENERAL_ERRORS"}, {20118, "DRV_ERROR_UNMDL"},
            {20050, "DRV_INVALID_AUX"}, {20119, "DRV_ERROR_BUFFSIZE"},
            {20051, "DRV_COF_NOTLOADED"}, {20121, "DRV_ERROR_NOHANDLE"},
            {20052, "DRV_FPGAPROG"}, {20130, "DRV_GATING_NOT_AVAILABLE"},
            {20053, "DRV_FLEXERROR"}, {20131, "DRV_FPGA_VOLTAGE_ERROR"},
            {20054, "DRV_GPIBERROR"}, {20099, "DRV_BINNING_ERROR"},
            {20055, "ERROR_DMA_UPLOAD"}, {20100, "DRV_INVALID_AMPLIFIER"},
            {20064, "DRV_DATATYPE"}, {20101, "DRV_INVALID_COUNTCONVERT_MODE"}
        };
    };
}