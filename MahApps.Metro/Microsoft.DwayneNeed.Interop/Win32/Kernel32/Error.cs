using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DwayneNeed.Win32.Kernel32
{
    public enum Error : int
    {
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        SUCCESS = 0,

        /// <summary>
        /// Incorrect function.
        /// </summary>
        INVALID_FUNCTION = 1,    // dderror

        /// <summary>
        /// The system cannot find the file specified.
        /// </summary>
        FILE_NOT_FOUND = 2,

        /// <summary>
        /// The system cannot find the path specified.
        /// </summary>
        PATH_NOT_FOUND = 3,

        /// <summary>
        /// The system cannot open the file.
        /// </summary>
        TOO_MANY_OPEN_FILES = 4,

        /// <summary>
        /// Access is denied.
        /// </summary>
        ACCESS_DENIED = 5,

        /// <summary>
        /// The handle is invalid.
        /// </summary>
        INVALID_HANDLE = 6,

        /// <summary>
        /// The storage control blocks were destroyed.
        /// </summary>
        ARENA_TRASHED = 7,

        /// <summary>
        /// Not enough storage is available to process this command.
        /// </summary>
        NOT_ENOUGH_MEMORY = 8,    // dderror

        /// <summary>
        /// The storage control block address is invalid.
        /// </summary>
        INVALID_BLOCK = 9,

        /// <summary>
        /// The environment is incorrect.
        /// </summary>
        BAD_ENVIRONMENT = 10,

        /// <summary>
        /// An attempt was made to load a program with an incorrect format.
        /// </summary>
        BAD_FORMAT = 11,

        /// <summary>
        /// The access code is invalid.
        /// </summary>
        INVALID_ACCESS = 12,

        /// <summary>
        /// The data is invalid.
        /// </summary>
        INVALID_DATA = 13,

        /// <summary>
        /// Not enough storage is available to complete this operation.
        /// </summary>
        OUTOFMEMORY = 14,

        /// <summary>
        /// The system cannot find the drive specified.
        /// </summary>
        INVALID_DRIVE = 15,

        /// <summary>
        /// The directory cannot be removed.
        /// </summary>
        CURRENT_DIRECTORY = 16,

        /// <summary>
        /// The system cannot move the file to a different disk drive.
        /// </summary>
        NOT_SAME_DEVICE = 17,

        /// <summary>
        /// There are no more files.
        /// </summary>
        NO_MORE_FILES = 18,

        /// <summary>
        /// The media is write protected.
        /// </summary>
        WRITE_PROTECT = 19,

        /// <summary>
        /// The system cannot find the device specified.
        /// </summary>
        BAD_UNIT = 20,

        /// <summary>
        /// The device is not ready.
        /// </summary>
        NOT_READY = 21,

        /// <summary>
        /// The device does not recognize the command.
        /// </summary>
        BAD_COMMAND = 22,

        /// <summary>
        /// Data error (cyclic redundancy check).
        /// </summary>
        CRC = 23,

        /// <summary>
        /// The program issued a command but the command length is incorrect.
        /// </summary>
        BAD_LENGTH = 24,

        /// <summary>
        /// The drive cannot locate a specific area or track on the disk.
        /// </summary>
        SEEK = 25,

        /// <summary>
        /// The specified disk or diskette cannot be accessed.
        /// </summary>
        NOT_DOS_DISK = 26,

        /// <summary>
        /// The drive cannot find the sector requested.
        /// </summary>
        SECTOR_NOT_FOUND = 27,

        /// <summary>
        /// The printer is out of paper.
        /// </summary>
        OUT_OF_PAPER = 28,

        /// <summary>
        /// The system cannot write to the specified device.
        /// </summary>
        WRITE_FAULT = 29,

        /// <summary>
        /// The system cannot read from the specified device.
        /// </summary>
        READ_FAULT = 30,

        /// <summary>
        /// A device attached to the system is not functioning.
        /// </summary>
        GEN_FAILURE = 31,

        /// <summary>
        /// The process cannot access the file because it is being used by another process.
        /// </summary>
        SHARING_VIOLATION = 32,

        /// <summary>
        /// The process cannot access the file because another process has locked a portion of the file.
        /// </summary>
        LOCK_VIOLATION = 33,

        /// <summary>
        /// The wrong diskette is in the drive.
        /// </summary> Insert %2 (Volume Serial Number: %3) into drive %1.
        //
        WRONG_DISK = 34,

        /// <summary>
        /// Too many files opened for sharing.
        /// </summary>
        SHARING_BUFFER_EXCEEDED = 36,

        /// <summary>
        /// Reached the end of the file.
        /// </summary>
        HANDLE_EOF = 38,

        /// <summary>
        /// The disk is full.
        /// </summary>
        HANDLE_DISK_FULL = 39,

        /// <summary>
        /// The request is not supported.
        /// </summary>
        NOT_SUPPORTED = 50,

        /// <summary>
        /// Windows cannot find the network path. Verify that the network path is correct and the destination computer is not busy or turned off. If Windows still cannot find the network path, contact your network administrator.
        /// </summary>
        REM_NOT_LIST = 51,

        /// <summary>
        /// You were not connected because a duplicate name exists on the network. If joining a domain, go to System in Control Panel to change the computer name and try again. If joining a workgroup, choose another workgroup name.
        /// </summary>
        DUP_NAME = 52,

        /// <summary>
        /// The network path was not found.
        /// </summary>
        BAD_NETPATH = 53,

        /// <summary>
        /// The network is busy.
        /// </summary>
        NETWORK_BUSY = 54,

        /// <summary>
        /// The specified network resource or device is no longer available.
        /// </summary>
        DEV_NOT_EXIST = 55,    // dderror

        /// <summary>
        /// The network BIOS command limit has been reached.
        /// </summary>
        TOO_MANY_CMDS = 56,

        /// <summary>
        /// A network adapter hardware error occurred.
        /// </summary>
        ADAP_HDW_ERR = 57,

        /// <summary>
        /// The specified server cannot perform the requested operation.
        /// </summary>
        BAD_NET_RESP = 58,

        /// <summary>
        /// An unexpected network error occurred.
        /// </summary>
        UNEXP_NET_ERR = 59,

        /// <summary>
        /// The remote adapter is not compatible.
        /// </summary>
        BAD_REM_ADAP = 60,

        /// <summary>
        /// The printer queue is full.
        /// </summary>
        PRINTQ_FULL = 61,

        /// <summary>
        /// Space to store the file waiting to be printed is not available on the server.
        /// </summary>
        NO_SPOOL_SPACE = 62,

        /// <summary>
        /// Your file waiting to be printed was deleted.
        /// </summary>
        PRINT_CANCELLED = 63,

        /// <summary>
        /// The specified network name is no longer available.
        /// </summary>
        NETNAME_DELETED = 64,

        /// <summary>
        /// Network access is denied.
        /// </summary>
        NETWORK_ACCESS_DENIED = 65,

        /// <summary>
        /// The network resource type is not correct.
        /// </summary>
        BAD_DEV_TYPE = 66,

        /// <summary>
        /// The network name cannot be found.
        /// </summary>
        BAD_NET_NAME = 67,

        /// <summary>
        /// The name limit for the local computer network adapter card was exceeded.
        /// </summary>
        TOO_MANY_NAMES = 68,

        /// <summary>
        /// The network BIOS session limit was exceeded.
        /// </summary>
        TOO_MANY_SESS = 69,

        /// <summary>
        /// The remote server has been paused or is in the process of being started.
        /// </summary>
        SHARING_PAUSED = 70,

        /// <summary>
        /// No more connections can be made to this remote computer at this time because there are already as many connections as the computer can accept.
        /// </summary>
        REQ_NOT_ACCEP = 71,

        /// <summary>
        /// The specified printer or disk device has been paused.
        /// </summary>
        REDIR_PAUSED = 72,

        /// <summary>
        /// The file exists.
        /// </summary>
        FILE_EXISTS = 80,

        /// <summary>
        /// The directory or file cannot be created.
        /// </summary>
        CANNOT_MAKE = 82,

        /// <summary>
        /// Fail on INT 24.
        /// </summary>
        FAIL_I24 = 83,

        /// <summary>
        /// Storage to process this request is not available.
        /// </summary>
        OUT_OF_STRUCTURES = 84,

        /// <summary>
        /// The local device name is already in use.
        /// </summary>
        ALREADY_ASSIGNED = 85,

        /// <summary>
        /// The specified network password is not correct.
        /// </summary>
        INVALID_PASSWORD = 86,

        /// <summary>
        /// The parameter is incorrect.
        /// </summary>
        INVALID_PARAMETER = 87,    // dderror

        /// <summary>
        /// A write fault occurred on the network.
        /// </summary>
        NET_WRITE_FAULT = 88,

        /// <summary>
        /// The system cannot start another process at this time.
        /// </summary>
        NO_PROC_SLOTS = 89,

        /// <summary>
        /// Cannot create another system semaphore.
        /// </summary>
        TOO_MANY_SEMAPHORES = 100,

        /// <summary>
        /// The exclusive semaphore is owned by another process.
        /// </summary>
        EXCL_SEM_ALREADY_OWNED = 101,

        /// <summary>
        /// The semaphore is set and cannot be closed.
        /// </summary>
        SEM_IS_SET = 102,

        /// <summary>
        /// The semaphore cannot be set again.
        /// </summary>
        TOO_MANY_SEM_REQUESTS = 103,

        /// <summary>
        /// Cannot request exclusive semaphores at interrupt time.
        /// </summary>
        INVALID_AT_INTERRUPT_TIME = 104,

        /// <summary>
        /// The previous ownership of this semaphore has ended.
        /// </summary>
        SEM_OWNER_DIED = 105,

        /// <summary>
        /// Insert the diskette for drive %1.
        /// </summary>
        SEM_USER_LIMIT = 106,

        /// <summary>
        /// The program stopped because an alternate diskette was not inserted.
        /// </summary>
        DISK_CHANGE = 107,

        /// <summary>
        /// The disk is in use or locked by another process.
        /// </summary>
        DRIVE_LOCKED = 108,

        /// <summary>
        /// The pipe has been ended.
        /// </summary>
        BROKEN_PIPE = 109,

        /// <summary>
        /// The system cannot open the device or file specified.
        /// </summary>
        OPEN_FAILED = 110,

        /// <summary>
        /// The file name is too long.
        /// </summary>
        BUFFER_OVERFLOW = 111,

        /// <summary>
        /// There is not enough space on the disk.
        /// </summary>
        DISK_FULL = 112,

        /// <summary>
        /// No more internal file identifiers available.
        /// </summary>
        NO_MORE_SEARCH_HANDLES = 113,

        /// <summary>
        /// The target internal file identifier is incorrect.
        /// </summary>
        INVALID_TARGET_HANDLE = 114,

        /// <summary>
        /// The IOCTL call made by the application program is not correct.
        /// </summary>
        INVALID_CATEGORY = 117,

        /// <summary>
        /// The verify-on-write switch parameter value is not correct.
        /// </summary>
        INVALID_VERIFY_SWITCH = 118,

        /// <summary>
        /// The system does not support the command requested.
        /// </summary>
        BAD_DRIVER_LEVEL = 119,

        /// <summary>
        /// This function is not supported on this system.
        /// </summary>
        CALL_NOT_IMPLEMENTED = 120,

        /// <summary>
        /// The semaphore timeout period has expired.
        /// </summary>
        SEM_TIMEOUT = 121,

        /// <summary>
        /// The data area passed to a system call is too small.
        /// </summary>
        INSUFFICIENT_BUFFER = 122,    // dderror

        /// <summary>
        /// The filename, directory name, or volume label syntax is incorrect.
        /// </summary>
        INVALID_NAME = 123,    // dderror

        /// <summary>
        /// The system call level is not correct.
        /// </summary>
        INVALID_LEVEL = 124,

        /// <summary>
        /// The disk has no volume label.
        /// </summary>
        NO_VOLUME_LABEL = 125,

        /// <summary>
        /// The specified module could not be found.
        /// </summary>
        MOD_NOT_FOUND = 126,

        /// <summary>
        /// The specified procedure could not be found.
        /// </summary>
        PROC_NOT_FOUND = 127,

        /// <summary>
        /// There are no child processes to wait for.
        /// </summary>
        WAIT_NO_CHILDREN = 128,

        /// <summary>
        /// The %1 application cannot be run in Win32 mode.
        /// </summary>
        CHILD_NOT_COMPLETE = 129,

        /// <summary>
        /// Attempt to use a file handle to an open disk partition for an operation other than raw disk I/O.
        /// </summary>
        DIRECT_ACCESS_HANDLE = 130,

        /// <summary>
        /// An attempt was made to move the file pointer before the beginning of the file.
        /// </summary>
        NEGATIVE_SEEK = 131,

        /// <summary>
        /// The file pointer cannot be set on the specified device or file.
        /// </summary>
        SEEK_ON_DEVICE = 132,

        /// <summary>
        /// A JOIN or SUBST command cannot be used for a drive that contains previously joined drives.
        /// </summary>
        IS_JOIN_TARGET = 133,

        /// <summary>
        /// An attempt was made to use a JOIN or SUBST command on a drive that has already been joined.
        /// </summary>
        IS_JOINED = 134,

        /// <summary>
        /// An attempt was made to use a JOIN or SUBST command on a drive that has already been substituted.
        /// </summary>
        IS_SUBSTED = 135,

        /// <summary>
        /// The system tried to delete the JOIN of a drive that is not joined.
        /// </summary>
        NOT_JOINED = 136,

        /// <summary>
        /// The system tried to delete the substitution of a drive that is not substituted.
        /// </summary>
        NOT_SUBSTED = 137,

        /// <summary>
        /// The system tried to join a drive to a directory on a joined drive.
        /// </summary>
        JOIN_TO_JOIN = 138,

        /// <summary>
        /// The system tried to substitute a drive to a directory on a substituted drive.
        /// </summary>
        SUBST_TO_SUBST = 139,

        /// <summary>
        /// The system tried to join a drive to a directory on a substituted drive.
        /// </summary>
        JOIN_TO_SUBST = 140,

        /// <summary>
        /// The system tried to SUBST a drive to a directory on a joined drive.
        /// </summary>
        SUBST_TO_JOIN = 141,

        /// <summary>
        /// The system cannot perform a JOIN or SUBST at this time.
        /// </summary>
        BUSY_DRIVE = 142,

        /// <summary>
        /// The system cannot join or substitute a drive to or for a directory on the same drive.
        /// </summary>
        SAME_DRIVE = 143,

        /// <summary>
        /// The directory is not a subdirectory of the root directory.
        /// </summary>
        DIR_NOT_ROOT = 144,

        /// <summary>
        /// The directory is not empty.
        /// </summary>
        DIR_NOT_EMPTY = 145,

        /// <summary>
        /// The path specified is being used in a substitute.
        /// </summary>
        IS_SUBST_PATH = 146,

        /// <summary>
        /// Not enough resources are available to process this command.
        /// </summary>
        IS_JOIN_PATH = 147,

        /// <summary>
        /// The path specified cannot be used at this time.
        /// </summary>
        PATH_BUSY = 148,

        /// <summary>
        /// An attempt was made to join or substitute a drive for which a directory on the drive is the target of a previous substitute.
        /// </summary>
        IS_SUBST_TARGET = 149,

        /// <summary>
        /// System trace information was not specified in your CONFIG.SYS file, or tracing is disallowed.
        /// </summary>
        SYSTEM_TRACE = 150,

        /// <summary>
        /// The number of specified semaphore events for DosMuxSemWait is not correct.
        /// </summary>
        INVALID_EVENT_COUNT = 151,

        /// <summary>
        /// DosMuxSemWait did not execute; too many semaphores are already set.
        /// </summary>
        TOO_MANY_MUXWAITERS = 152,

        /// <summary>
        /// The DosMuxSemWait list is not correct.
        /// </summary>
        INVALID_LIST_FORMAT = 153,

        /// <summary>
        /// The volume label you entered exceeds the label character limit of the target file system.
        /// </summary>
        LABEL_TOO_LONG = 154,

        /// <summary>
        /// Cannot create another thread.
        /// </summary>
        TOO_MANY_TCBS = 155,

        /// <summary>
        /// The recipient process has refused the signal.
        /// </summary>
        SIGNAL_REFUSED = 156,

        /// <summary>
        /// The segment is already discarded and cannot be locked.
        /// </summary>
        DISCARDED = 157,

        /// <summary>
        /// The segment is already unlocked.
        /// </summary>
        NOT_LOCKED = 158,

        /// <summary>
        /// The address for the thread ID is not correct.
        /// </summary>
        BAD_THREADID_ADDR = 159,

        /// <summary>
        /// One or more arguments are not correct.
        /// </summary>
        BAD_ARGUMENTS = 160,

        /// <summary>
        /// The specified path is invalid.
        /// </summary>
        BAD_PATHNAME = 161,

        /// <summary>
        /// A signal is already pending.
        /// </summary>
        SIGNAL_PENDING = 162,

        /// <summary>
        /// No more threads can be created in the system.
        /// </summary>
        MAX_THRDS_REACHED = 164,

        /// <summary>
        /// Unable to lock a region of a file.
        /// </summary>
        LOCK_FAILED = 167,

        /// <summary>
        /// The requested resource is in use.
        /// </summary>
        BUSY = 170,    // dderror

        /// <summary>
        /// Device's command support detection is in progress.
        /// </summary>
        DEVICE_SUPPORT_IN_PROGRESS = 171,

        /// <summary>
        /// A lock request was not outstanding for the supplied cancel region.
        /// </summary>
        CANCEL_VIOLATION = 173,

        /// <summary>
        /// The file system does not support atomic changes to the lock type.
        /// </summary>
        ATOMIC_LOCKS_NOT_SUPPORTED = 174,

        /// <summary>
        /// The system detected a segment number that was not correct.
        /// </summary>
        INVALID_SEGMENT_NUMBER = 180,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        INVALID_ORDINAL = 182,

        /// <summary>
        /// Cannot create a file when that file already exists.
        /// </summary>
        ALREADY_EXISTS = 183,

        /// <summary>
        /// The flag passed is not correct.
        /// </summary>
        INVALID_FLAG_NUMBER = 186,

        /// <summary>
        /// The specified system semaphore name was not found.
        /// </summary>
        SEM_NOT_FOUND = 187,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        INVALID_STARTING_CODESEG = 188,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        INVALID_STACKSEG = 189,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        INVALID_MODULETYPE = 190,

        /// <summary>
        /// Cannot run %1 in Win32 mode.
        /// </summary>
        INVALID_EXE_SIGNATURE = 191,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        EXE_MARKED_INVALID = 192,

        /// <summary>
        /// %1 is not a valid Win32 application.
        /// </summary>
        BAD_EXE_FORMAT = 193,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ITERATED_DATA_EXCEEDS_64k = 194,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        INVALID_MINALLOCSIZE = 195,

        /// <summary>
        /// The operating system cannot run this application program.
        /// </summary>
        DYNLINK_FROM_INVALID_RING = 196,

        /// <summary>
        /// The operating system is not presently configured to run this application.
        /// </summary>
        IOPL_NOT_ENABLED = 197,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        INVALID_SEGDPL = 198,

        /// <summary>
        /// The operating system cannot run this application program.
        /// </summary>
        AUTODATASEG_EXCEEDS_64k = 199,

        /// <summary>
        /// The code segment cannot be greater than or equal to 64K.
        /// </summary>
        RING2SEG_MUST_BE_MOVABLE = 200,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        RELOC_CHAIN_XEEDS_SEGLIM = 201,

        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        INFLOOP_IN_RELOC_CHAIN = 202,

        /// <summary>
        /// The system could not find the environment option that was entered.
        /// </summary>
        ENVVAR_NOT_FOUND = 203,

        /// <summary>
        /// No process in the command subtree has a signal handler.
        /// </summary>
        NO_SIGNAL_SENT = 205,

        /// <summary>
        /// The filename or extension is too long.
        /// </summary>
        FILENAME_EXCED_RANGE = 206,

        /// <summary>
        /// The ring 2 stack is in use.
        /// </summary>
        RING2_STACK_IN_USE = 207,

        /// <summary>
        /// The global filename characters, * or ?, are entered incorrectly or too many global filename characters are specified.
        /// </summary>
        META_EXPANSION_TOO_LONG = 208,

        /// <summary>
        /// The signal being posted is not correct.
        /// </summary>
        INVALID_SIGNAL_NUMBER = 209,

        /// <summary>
        /// The signal handler cannot be set.
        /// </summary>
        THREAD_1_INACTIVE = 210,

        /// <summary>
        /// The segment is locked and cannot be reallocated.
        /// </summary>
        LOCKED = 212,

        /// <summary>
        /// Too many dynamic-link modules are attached to this program or dynamic-link module.
        /// </summary>
        TOO_MANY_MODULES = 214,

        /// <summary>
        /// Cannot nest calls to LoadModule.
        /// </summary>
        NESTING_NOT_ALLOWED = 215,

        /// <summary>
        /// This version of %1 is not compatible with the version of Windows you're running. Check your computer's system information and then contact the software publisher.
        /// </summary>
        EXE_MACHINE_TYPE_MISMATCH = 216,

        /// <summary>
        /// The image file %1 is signed, unable to modify.
        /// </summary>
        EXE_CANNOT_MODIFY_SIGNED_BINARY = 217,

        /// <summary>
        /// The image file %1 is strong signed, unable to modify.
        /// </summary>
        EXE_CANNOT_MODIFY_STRONG_SIGNED_BINARY = 218,

        /// <summary>
        /// This file is checked out or locked for editing by another user.
        /// </summary>
        FILE_CHECKED_OUT = 220,

        /// <summary>
        /// The file must be checked out before saving changes.
        /// </summary>
        CHECKOUT_REQUIRED = 221,

        /// <summary>
        /// The file type being saved or retrieved has been blocked.
        /// </summary>
        BAD_FILE_TYPE = 222,

        /// <summary>
        /// The file size exceeds the limit allowed and cannot be saved.
        /// </summary>
        FILE_TOO_LARGE = 223,

        /// <summary>
        /// Access Denied. Before opening files in this location, you must first add the web site to your trusted sites list, browse to the web site, and select the option to login automatically.
        /// </summary>
        FORMS_AUTH_REQUIRED = 224,

        /// <summary>
        /// Operation did not complete successfully because the file contains a virus or potentially unwanted software.
        /// </summary>
        VIRUS_INFECTED = 225,

        /// <summary>
        /// This file contains a virus or potentially unwanted software and cannot be opened. Due to the nature of this virus or potentially unwanted software, the file has been removed from this location.
        /// </summary>
        VIRUS_DELETED = 226,

        /// <summary>
        /// The pipe is local.
        /// </summary>
        PIPE_LOCAL = 229,

        /// <summary>
        /// The pipe state is invalid.
        /// </summary>
        BAD_PIPE = 230,

        /// <summary>
        /// All pipe instances are busy.
        /// </summary>
        PIPE_BUSY = 231,

        /// <summary>
        /// The pipe is being closed.
        /// </summary>
        NO_DATA = 232,

        /// <summary>
        /// No process is on the other end of the pipe.
        /// </summary>
        PIPE_NOT_CONNECTED = 233,

        /// <summary>
        /// More data is available.
        /// </summary>
        MORE_DATA = 234,    // dderror

        /// <summary>
        /// The session was canceled.
        /// </summary>
        VC_DISCONNECTED = 240,

        /// <summary>
        /// The specified extended attribute name was invalid.
        /// </summary>
        INVALID_EA_NAME = 254,

        /// <summary>
        /// The extended attributes are inconsistent.
        /// </summary>
        EA_LIST_INCONSISTENT = 255,

        /// <summary>
        /// The wait operation timed out.
        /// </summary>
        WAIT_TIMEOUT = 258,    // dderror

        /// <summary>
        /// No more data is available.
        /// </summary>
        NO_MORE_ITEMS = 259,

        /// <summary>
        /// The copy functions cannot be used.
        /// </summary>
        CANNOT_COPY = 266,

        /// <summary>
        /// The directory name is invalid.
        /// </summary>
        DIRECTORY = 267,

        /// <summary>
        /// The extended attributes did not fit in the buffer.
        /// </summary>
        EAS_DIDNT_FIT = 275,

        /// <summary>
        /// The extended attribute file on the mounted file system is corrupt.
        /// </summary>
        EA_FILE_CORRUPT = 276,

        /// <summary>
        /// The extended attribute table file is full.
        /// </summary>
        EA_TABLE_FULL = 277,

        /// <summary>
        /// The specified extended attribute handle is invalid.
        /// </summary>
        INVALID_EA_HANDLE = 278,

        /// <summary>
        /// The mounted file system does not support extended attributes.
        /// </summary>
        EAS_NOT_SUPPORTED = 282,

        /// <summary>
        /// Attempt to release mutex not owned by caller.
        /// </summary>
        NOT_OWNER = 288,

        /// <summary>
        /// Too many posts were made to a semaphore.
        /// </summary>
        TOO_MANY_POSTS = 298,

        /// <summary>
        /// Only part of a ReadProcessMemory or WriteProcessMemory request was completed.
        /// </summary>
        PARTIAL_COPY = 299,

        /// <summary>
        /// The oplock request is denied.
        /// </summary>
        OPLOCK_NOT_GRANTED = 300,

        /// <summary>
        /// An invalid oplock acknowledgment was received by the system.
        /// </summary>
        INVALID_OPLOCK_PROTOCOL = 301,

        /// <summary>
        /// The volume is too fragmented to complete this operation.
        /// </summary>
        DISK_TOO_FRAGMENTED = 302,

        /// <summary>
        /// The file cannot be opened because it is in the process of being deleted.
        /// </summary>
        DELETE_PENDING = 303,

        /// <summary>
        /// Short name settings may not be changed on this volume due to the global registry setting.
        /// </summary>
        INCOMPATIBLE_WITH_GLOBAL_SHORT_NAME_REGISTRY_SETTING = 304,

        /// <summary>
        /// Short names are not enabled on this volume.
        /// </summary>
        SHORT_NAMES_NOT_ENABLED_ON_VOLUME = 305,

        /// <summary>
        /// The security stream for the given volume is in an inconsistent state.
        /// </summary> Please run CHKDSK on the volume.
        //
        SECURITY_STREAM_IS_INCONSISTENT = 306,

        /// <summary>
        /// A requested file lock operation cannot be processed due to an invalid byte range.
        /// </summary>
        INVALID_LOCK_RANGE = 307,

        /// <summary>
        /// The subsystem needed to support the image type is not present.
        /// </summary>
        IMAGE_SUBSYSTEM_NOT_PRESENT = 308,

        /// <summary>
        /// The specified file already has a notification GUID associated with it.
        /// </summary>
        NOTIFICATION_GUID_ALREADY_DEFINED = 309,

        /// <summary>
        /// An invalid exception handler routine has been detected.
        /// </summary>
        INVALID_EXCEPTION_HANDLER = 310,

        /// <summary>
        /// Duplicate privileges were specified for the token.
        /// </summary>
        DUPLICATE_PRIVILEGES = 311,

        /// <summary>
        /// No ranges for the specified operation were able to be processed.
        /// </summary>
        NO_RANGES_PROCESSED = 312,

        /// <summary>
        /// Operation is not allowed on a file system internal file.
        /// </summary>
        NOT_ALLOWED_ON_SYSTEM_FILE = 313,

        /// <summary>
        /// The physical resources of this disk have been exhausted.
        /// </summary>
        DISK_RESOURCES_EXHAUSTED = 314,

        /// <summary>
        /// The token representing the data is invalid.
        /// </summary>
        INVALID_TOKEN = 315,

        /// <summary>
        /// The device does not support the command feature.
        /// </summary>
        DEVICE_FEATURE_NOT_SUPPORTED = 316,

        /// <summary>
        /// The system cannot find message text for message number 0x%1 in the message file for %2.
        /// </summary>
        MR_MID_NOT_FOUND = 317,

        /// <summary>
        /// The scope specified was not found.
        /// </summary>
        SCOPE_NOT_FOUND = 318,

        /// <summary>
        /// The Central Access Policy specified is not defined on the target machine.
        /// </summary>
        UNDEFINED_SCOPE = 319,

        /// <summary>
        /// The Central Access Policy obtained from Active Directory is invalid.
        /// </summary>
        INVALID_CAP = 320,

        /// <summary>
        /// The device is unreachable.
        /// </summary>
        DEVICE_UNREACHABLE = 321,

        /// <summary>
        /// The target device has insufficient resources to complete the operation.
        /// </summary>
        DEVICE_NO_RESOURCES = 322,

        /// <summary>
        /// A data integrity checksum error occurred. Data in the file stream is corrupt.
        /// </summary>
        DATA_CHECKSUM_ERROR = 323,

        /// <summary>
        /// An attempt was made to modify both a KERNEL and normal Extended Attribute (EA) in the same operation.
        /// </summary>
        INTERMIXED_KERNEL_EA_OPERATION = 324,

        /// <summary>
        /// Device does not support file-level TRIM.
        /// </summary>
        FILE_LEVEL_TRIM_NOT_SUPPORTED = 326,

        /// <summary>
        /// The command specified a data offset that does not align to the device's granularity/alignment.
        /// </summary>
        OFFSET_ALIGNMENT_VIOLATION = 327,

        /// <summary>
        /// The command specified an invalid field in its parameter list.
        /// </summary>
        INVALID_FIELD_IN_PARAMETER_LIST = 328,

        /// <summary>
        /// An operation is currently in progress with the device.
        /// </summary>
        OPERATION_IN_PROGRESS = 329,

        /// <summary>
        /// An attempt was made to send down the command via an invalid path to the target device.
        /// </summary>
        BAD_DEVICE_PATH = 330,

        /// <summary>
        /// The command specified a number of descriptors that exceeded the maximum supported by the device.
        /// </summary>
        TOO_MANY_DESCRIPTORS = 331,

        /// <summary>
        /// Scrub is disabled on the specified file.
        /// </summary>
        SCRUB_DATA_DISABLED = 332,

        /// <summary>
        /// The storage device does not provide redundancy.
        /// </summary>
        NOT_REDUNDANT_STORAGE = 333,

        /// <summary>
        /// An operation is not supported on a resident file.
        /// </summary>
        RESIDENT_FILE_NOT_SUPPORTED = 334,

        /// <summary>
        /// An operation is not supported on a compressed file.
        /// </summary>
        COMPRESSED_FILE_NOT_SUPPORTED = 335,

        /// <summary>
        /// An operation is not supported on a directory.
        /// </summary>
        DIRECTORY_NOT_SUPPORTED = 336,

        /// <summary>
        /// The specified copy of the requested data could not be read.
        /// </summary>
        NOT_READ_FROM_COPY = 337,

        /// <summary>
        /// The specified data could not be written to any of the copies.
        /// </summary>
        FT_WRITE_FAILURE = 338,

        /// <summary>
        /// One or more copies of data on this device may be out of sync. No writes may be performed until a data integrity scan is completed.
        /// </summary>
        FT_DI_SCAN_REQUIRED = 339,

        /// <summary>
        /// The supplied kernel information version is invalid.
        /// </summary>
        INVALID_KERNEL_INFO_VERSION = 340,

        /// <summary>
        /// The supplied PEP information version is invalid.
        /// </summary>
        INVALID_PEP_INFO_VERSION = 341,

        /// <summary>
        /// **** Available SYSTEM error codes ****
        /// </summary>
        /// <summary>
        /// No action was taken as a system reboot is required.
        /// </summary>
        FAIL_NOACTION_REBOOT = 350,

        /// <summary>
        /// The shutdown operation failed.
        /// </summary>
        FAIL_SHUTDOWN = 351,

        /// <summary>
        /// The restart operation failed.
        /// </summary>
        FAIL_RESTART = 352,

        /// <summary>
        /// The maximum number of sessions has been reached.
        /// </summary>
        MAX_SESSIONS_REACHED = 353,

        /// <summary>
        /// **** Available SYSTEM error codes ****
        /// </summary>
        /// <summary>
        /// The thread is already in background processing mode.
        /// </summary>
        THREAD_MODE_ALREADY_BACKGROUND = 400,

        /// <summary>
        /// The thread is not in background processing mode.
        /// </summary>
        THREAD_MODE_NOT_BACKGROUND = 401,

        /// <summary>
        /// The process is already in background processing mode.
        /// </summary>
        PROCESS_MODE_ALREADY_BACKGROUND = 402,

        /// <summary>
        /// The process is not in background processing mode.
        /// </summary>
        PROCESS_MODE_NOT_BACKGROUND = 403,

        /// <summary>
        /// **** Available SYSTEM error codes ****
        /// </summary>
        /// <summary>
        /// Attempt to access invalid address.
        /// </summary>
        INVALID_ADDRESS = 487,

        /// <summary>
        /// **** Available SYSTEM error codes ****
        /// </summary>
        /// <summary>
        /// User profile cannot be loaded.
        /// </summary>
        USER_PROFILE_LOAD = 500,

        /// <summary>
        /// **** Available SYSTEM error codes ****
        /// </summary>
        /// <summary>
        /// Arithmetic result exceeded 32 bits.
        /// </summary>
        ARITHMETIC_OVERFLOW = 534,

        /// <summary>
        /// There is a process on other end of the pipe.
        /// </summary>
        PIPE_CONNECTED = 535,

        /// <summary>
        /// Waiting for a process to open the other end of the pipe.
        /// </summary>
        PIPE_LISTENING = 536,

        /// <summary>
        /// Application verifier has found an error in the current process.
        /// </summary>
        VERIFIER_STOP = 537,

        /// <summary>
        /// An error occurred in the ABIOS subsystem.
        /// </summary>
        ABIOS_ERROR = 538,

        /// <summary>
        /// A warning occurred in the WX86 subsystem.
        /// </summary>
        WX86_WARNING = 539,

        /// <summary>
        /// An error occurred in the WX86 subsystem.
        /// </summary>
        WX86_ERROR = 540,

        /// <summary>
        /// An attempt was made to cancel or set a timer that has an associated APC and the subject thread is not the thread that originally set the timer with an associated APC routine.
        /// </summary>
        TIMER_NOT_CANCELED = 541,

        /// <summary>
        /// Unwind exception code.
        /// </summary>
        UNWIND = 542,

        /// <summary>
        /// An invalid or unaligned stack was encountered during an unwind operation.
        /// </summary>
        BAD_STACK = 543,

        /// <summary>
        /// An invalid unwind target was encountered during an unwind operation.
        /// </summary>
        INVALID_UNWIND_TARGET = 544,

        /// <summary>
        /// Invalid Object Attributes specified to NtCreatePort or invalid Port Attributes specified to NtConnectPort
        /// </summary>
        INVALID_PORT_ATTRIBUTES = 545,

        /// <summary>
        /// Length of message passed to NtRequestPort or NtRequestWaitReplyPort was longer than the maximum message allowed by the port.
        /// </summary>
        PORT_MESSAGE_TOO_LONG = 546,

        /// <summary>
        /// An attempt was made to lower a quota limit below the current usage.
        /// </summary>
        INVALID_QUOTA_LOWER = 547,

        /// <summary>
        /// An attempt was made to attach to a device that was already attached to another device.
        /// </summary>
        DEVICE_ALREADY_ATTACHED = 548,

        /// <summary>
        /// An attempt was made to execute an instruction at an unaligned address and the host system does not support unaligned instruction references.
        /// </summary>
        INSTRUCTION_MISALIGNMENT = 549,

        /// <summary>
        /// Profiling not started.
        /// </summary>
        PROFILING_NOT_STARTED = 550,

        /// <summary>
        /// Profiling not stopped.
        /// </summary>
        PROFILING_NOT_STOPPED = 551,

        /// <summary>
        /// The passed ACL did not contain the minimum required information.
        /// </summary>
        COULD_NOT_INTERPRET = 552,

        /// <summary>
        /// The number of active profiling objects is at the maximum and no more may be started.
        /// </summary>
        PROFILING_AT_LIMIT = 553,

        /// <summary>
        /// Used to indicate that an operation cannot continue without blocking for I/O.
        /// </summary>
        CANT_WAIT = 554,

        /// <summary>
        /// Indicates that a thread attempted to terminate itself by default (called NtTerminateThread with NULL) and it was the last thread in the current process.
        /// </summary>
        CANT_TERMINATE_SELF = 555,

        /// <summary>
        /// If an MM error is returned which is not defined in the standard FsRtl filter, it is converted to one of the following errors which is guaranteed to be in the filter.
        /// </summary> In this case information is lost, however, the filter correctly handles the exception.
        //
        UNEXPECTED_MM_CREATE_ERR = 556,

        /// <summary>
        /// If an MM error is returned which is not defined in the standard FsRtl filter, it is converted to one of the following errors which is guaranteed to be in the filter.
        /// </summary> In this case information is lost, however, the filter correctly handles the exception.
        //
        UNEXPECTED_MM_MAP_ERROR = 557,

        /// <summary>
        /// If an MM error is returned which is not defined in the standard FsRtl filter, it is converted to one of the following errors which is guaranteed to be in the filter.
        /// </summary> In this case information is lost, however, the filter correctly handles the exception.
        //
        UNEXPECTED_MM_EXTEND_ERR = 558,

        /// <summary>
        /// A malformed function table was encountered during an unwind operation.
        /// </summary>
        BAD_FUNCTION_TABLE = 559,

        /// <summary>
        /// Indicates that an attempt was made to assign protection to a file system file or directory and one of the SIDs in the security descriptor could not be translated into a GUID that could be stored by the file system.
        /// </summary> This causes the protection attempt to fail, which may cause a file creation attempt to fail.
        //
        NO_GUID_TRANSLATION = 560,

        /// <summary>
        /// Indicates that an attempt was made to grow an LDT by setting its size, or that the size was not an even number of selectors.
        /// </summary>
        INVALID_LDT_SIZE = 561,

        /// <summary>
        /// Indicates that the starting value for the LDT information was not an integral multiple of the selector size.
        /// </summary>
        INVALID_LDT_OFFSET = 563,

        /// <summary>
        /// Indicates that the user supplied an invalid descriptor when trying to set up Ldt descriptors.
        /// </summary>
        INVALID_LDT_DESCRIPTOR = 564,

        /// <summary>
        /// Indicates a process has too many threads to perform the requested action. For example, assignment of a primary token may only be performed when a process has zero or one threads.
        /// </summary>
        TOO_MANY_THREADS = 565,

        /// <summary>
        /// An attempt was made to operate on a thread within a specific process, but the thread specified is not in the process specified.
        /// </summary>
        THREAD_NOT_IN_PROCESS = 566,

        /// <summary>
        /// Page file quota was exceeded.
        /// </summary>
        PAGEFILE_QUOTA_EXCEEDED = 567,

        /// <summary>
        /// The Netlogon service cannot start because another Netlogon service running in the domain conflicts with the specified role.
        /// </summary>
        LOGON_SERVER_CONFLICT = 568,

        /// <summary>
        /// The SAM database on a Windows Server is significantly out of synchronization with the copy on the Domain Controller. A complete synchronization is required.
        /// </summary>
        SYNCHRONIZATION_REQUIRED = 569,

        /// <summary>
        /// The NtCreateFile API failed. This error should never be returned to an application, it is a place holder for the Windows Lan Manager Redirector to use in its internal error mapping routines.
        /// </summary>
        NET_OPEN_FAILED = 570,

        /// <summary>
        /// {Privilege Failed}
        /// </summary> The I/O permissions for the process could not be changed.
        //
        IO_PRIVILEGE_FAILED = 571,

        /// <summary>
        /// {Application Exit by CTRL+C}
        /// </summary> The application terminated as a result of a CTRL+C.
        //
        CONTROL_C_EXIT = 572,    // winnt

        /// <summary>
        /// {Missing System File}
        /// </summary> The required system file %hs is bad or missing.
        //
        MISSING_SYSTEMFILE = 573,

        /// <summary>
        /// {Application Error}
        /// </summary> The exception %s (0x%08lx) occurred in the application at location 0x%08lx.
        //
        UNHANDLED_EXCEPTION = 574,

        /// <summary>
        /// {Application Error}
        /// </summary> The application was unable to start correctly (0x%lx). Click OK to close the application.
        //
        APP_INIT_FAILURE = 575,

        /// <summary>
        /// {Unable to Create Paging File}
        /// </summary> The creation of the paging file %hs failed (%lx). The requested size was %ld.
        //
        PAGEFILE_CREATE_FAILED = 576,

        /// <summary>
        /// Windows cannot verify the digital signature for this file. A recent hardware or software change might have installed a file that is signed incorrectly or damaged, or that might be malicious software from an unknown source.
        /// </summary>
        INVALID_IMAGE_HASH = 577,

        /// <summary>
        /// {No Paging File Specified}
        /// </summary> No paging file was specified in the system configuration.
        //
        NO_PAGEFILE = 578,

        /// <summary>
        /// {EXCEPTION}
        /// </summary> A real-mode application issued a floating-point instruction and floating-point hardware is not present.
        //
        ILLEGAL_FLOAT_CONTEXT = 579,

        /// <summary>
        /// An event pair synchronization operation was performed using the thread specific client/server event pair object, but no event pair object was associated with the thread.
        /// </summary>
        NO_EVENT_PAIR = 580,

        /// <summary>
        /// A Windows Server has an incorrect configuration.
        /// </summary>
        DOMAIN_CTRLR_CONFIG_ERROR = 581,

        /// <summary>
        /// An illegal character was encountered. For a multi-byte character set this includes a lead byte without a succeeding trail byte. For the Unicode character set this includes the characters 0xFFFF and 0xFFFE.
        /// </summary>
        ILLEGAL_CHARACTER = 582,

        /// <summary>
        /// The Unicode character is not defined in the Unicode character set installed on the system.
        /// </summary>
        UNDEFINED_CHARACTER = 583,

        /// <summary>
        /// The paging file cannot be created on a floppy diskette.
        /// </summary>
        FLOPPY_VOLUME = 584,

        /// <summary>
        /// The system BIOS failed to connect a system interrupt to the device or bus for which the device is connected.
        /// </summary>
        BIOS_FAILED_TO_CONNECT_INTERRUPT = 585,

        /// <summary>
        /// This operation is only allowed for the Primary Domain Controller of the domain.
        /// </summary>
        BACKUP_CONTROLLER = 586,

        /// <summary>
        /// An attempt was made to acquire a mutant such that its maximum count would have been exceeded.
        /// </summary>
        MUTANT_LIMIT_EXCEEDED = 587,

        /// <summary>
        /// A volume has been accessed for which a file system driver is required that has not yet been loaded.
        /// </summary>
        FS_DRIVER_REQUIRED = 588,

        /// <summary>
        /// {Registry File Failure}
        /// </summary> The registry cannot load the hive (file):
        // %hs
        // or its log or alternate.
        // It is corrupt, absent, or not writable.
        //
        CANNOT_LOAD_REGISTRY_FILE = 589,

        /// <summary>
        /// {Unexpected Failure in DebugActiveProcess}
        /// </summary> An unexpected failure occurred while processing a DebugActiveProcess API request. You may choose OK to terminate the process, or Cancel to ignore the error.
        //
        DEBUG_ATTACH_FAILED = 590,

        /// <summary>
        /// {Fatal System Error}
        /// </summary> The %hs system process terminated unexpectedly with a status of 0x%08x (0x%08x 0x%08x).
        // The system has been shut down.
        //
        SYSTEM_PROCESS_TERMINATED = 591,

        /// <summary>
        /// {Data Not Accepted}
        /// </summary> The TDI client could not handle the data received during an indication.
        //
        DATA_NOT_ACCEPTED = 592,

        /// <summary>
        /// NTVDM encountered a hard error.
        /// </summary>
        VDM_HARD_ERROR = 593,

        /// <summary>
        /// {Cancel Timeout}
        /// </summary> The driver %hs failed to complete a cancelled I/O request in the allotted time.
        //
        DRIVER_CANCEL_TIMEOUT = 594,

        /// <summary>
        /// {Reply Message Mismatch}
        /// </summary> An attempt was made to reply to an LPC message, but the thread specified by the client ID in the message was not waiting on that message.
        //
        REPLY_MESSAGE_MISMATCH = 595,

        /// <summary>
        /// {Delayed Write Failed}
        /// </summary> Windows was unable to save all the data for the file %hs. The data has been lost.
        // This error may be caused by a failure of your computer hardware or network connection. Please try to save this file elsewhere.
        //
        LOST_WRITEBEHIND_DATA = 596,

        /// <summary>
        /// The parameter(s) passed to the server in the client/server shared memory window were invalid. Too much data may have been put in the shared memory window.
        /// </summary>
        CLIENT_SERVER_PARAMETERS_INVALID = 597,

        /// <summary>
        /// The stream is not a tiny stream.
        /// </summary>
        NOT_TINY_STREAM = 598,

        /// <summary>
        /// The request must be handled by the stack overflow code.
        /// </summary>
        STACK_OVERFLOW_READ = 599,

        /// <summary>
        /// Internal OFS status codes indicating how an allocation operation is handled. Either it is retried after the containing onode is moved or the extent stream is converted to a large stream.
        /// </summary>
        CONVERT_TO_LARGE = 600,

        /// <summary>
        /// The attempt to find the object found an object matching by ID on the volume but it is out of the scope of the handle used for the operation.
        /// </summary>
        FOUND_OUT_OF_SCOPE = 601,

        /// <summary>
        /// The bucket array must be grown. Retry transaction after doing so.
        /// </summary>
        ALLOCATE_BUCKET = 602,

        /// <summary>
        /// The user/kernel marshalling buffer has overflowed.
        /// </summary>
        MARSHALL_OVERFLOW = 603,

        /// <summary>
        /// The supplied variant structure contains invalid data.
        /// </summary>
        INVALID_VARIANT = 604,

        /// <summary>
        /// The specified buffer contains ill-formed data.
        /// </summary>
        BAD_COMPRESSION_BUFFER = 605,

        /// <summary>
        /// {Audit Failed}
        /// </summary> An attempt to generate a security audit failed.
        //
        AUDIT_FAILED = 606,

        /// <summary>
        /// The timer resolution was not previously set by the current process.
        /// </summary>
        TIMER_RESOLUTION_NOT_SET = 607,

        /// <summary>
        /// There is insufficient account information to log you on.
        /// </summary>
        INSUFFICIENT_LOGON_INFO = 608,

        /// <summary>
        /// {Invalid DLL Entrypoint}
        /// </summary> The dynamic link library %hs is not written correctly. The stack pointer has been left in an inconsistent state.
        // The entrypoint should be declared as WINAPI or STDCALL. Select YES to fail the DLL load. Select NO to continue execution. Selecting NO may cause the application to operate incorrectly.
        //
        BAD_DLL_ENTRYPOINT = 609,

        /// <summary>
        /// {Invalid Service Callback Entrypoint}
        /// </summary> The %hs service is not written correctly. The stack pointer has been left in an inconsistent state.
        // The callback entrypoint should be declared as WINAPI or STDCALL. Selecting OK will cause the service to continue operation. However, the service process may operate incorrectly.
        //
        BAD_SERVICE_ENTRYPOINT = 610,

        /// <summary>
        /// There is an IP address conflict with another system on the network
        /// </summary>
        IP_ADDRESS_CONFLICT1 = 611,

        /// <summary>
        /// There is an IP address conflict with another system on the network
        /// </summary>
        IP_ADDRESS_CONFLICT2 = 612,

        /// <summary>
        /// {Low On Registry Space}
        /// </summary> The system has reached the maximum size allowed for the system part of the registry. Additional storage requests will be ignored.
        //
        REGISTRY_QUOTA_LIMIT = 613,

        /// <summary>
        /// A callback return system service cannot be executed when no callback is active.
        /// </summary>
        NO_CALLBACK_ACTIVE = 614,

        /// <summary>
        /// The password provided is too short to meet the policy of your user account.
        /// </summary> Please choose a longer password.
        //
        PWD_TOO_SHORT = 615,

        /// <summary>
        /// The policy of your user account does not allow you to change passwords too frequently.
        /// </summary> This is done to prevent users from changing back to a familiar, but potentially discovered, password.
        // If you feel your password has been compromised then please contact your administrator immediately to have a new one assigned.
        //
        PWD_TOO_RECENT = 616,

        /// <summary>
        /// You have attempted to change your password to one that you have used in the past.
        /// </summary> The policy of your user account does not allow this. Please select a password that you have not previously used.
        //
        PWD_HISTORY_CONFLICT = 617,

        /// <summary>
        /// The specified compression format is unsupported.
        /// </summary>
        UNSUPPORTED_COMPRESSION = 618,

        /// <summary>
        /// The specified hardware profile configuration is invalid.
        /// </summary>
        INVALID_HW_PROFILE = 619,

        /// <summary>
        /// The specified Plug and Play registry device path is invalid.
        /// </summary>
        INVALID_PLUGPLAY_DEVICE_PATH = 620,

        /// <summary>
        /// The specified quota list is internally inconsistent with its descriptor.
        /// </summary>
        QUOTA_LIST_INCONSISTENT = 621,

        /// <summary>
        /// {Windows Evaluation Notification}
        /// </summary> The evaluation period for this installation of Windows has expired. This system will shutdown in 1 hour. To restore access to this installation of Windows, please upgrade this installation using a licensed distribution of this product.
        //
        EVALUATION_EXPIRATION = 622,

        /// <summary>
        /// {Illegal System DLL Relocation}
        /// </summary> The system DLL %hs was relocated in memory. The application will not run properly.
        // The relocation occurred because the DLL %hs occupied an address range reserved for Windows system DLLs. The vendor supplying the DLL should be contacted for a new DLL.
        //
        ILLEGAL_DLL_RELOCATION = 623,

        /// <summary>
        /// {DLL Initialization Failed}
        /// </summary> The application failed to initialize because the window station is shutting down.
        //
        DLL_INIT_FAILED_LOGOFF = 624,

        /// <summary>
        /// The validation process needs to continue on to the next step.
        /// </summary>
        VALIDATE_CONTINUE = 625,

        /// <summary>
        /// There are no more matches for the current index enumeration.
        /// </summary>
        NO_MORE_MATCHES = 626,

        /// <summary>
        /// The range could not be added to the range list because of a conflict.
        /// </summary>
        RANGE_LIST_CONFLICT = 627,

        /// <summary>
        /// The server process is running under a SID different than that required by client.
        /// </summary>
        SERVER_SID_MISMATCH = 628,

        /// <summary>
        /// A group marked use for deny only cannot be enabled.
        /// </summary>
        CANT_ENABLE_DENY_ONLY = 629,

        /// <summary>
        /// {EXCEPTION}
        /// </summary> Multiple floating point faults.
        //
        FLOAT_MULTIPLE_FAULTS = 630,    // winnt

        /// <summary>
        /// {EXCEPTION}
        /// </summary> Multiple floating point traps.
        //
        FLOAT_MULTIPLE_TRAPS = 631,    // winnt

        /// <summary>
        /// The requested interface is not supported.
        /// </summary>
        NOINTERFACE = 632,

        /// <summary>
        /// {System Standby Failed}
        /// </summary> The driver %hs does not support standby mode. Updating this driver may allow the system to go to standby mode.
        //
        DRIVER_FAILED_SLEEP = 633,

        /// <summary>
        /// The system file %1 has become corrupt and has been replaced.
        /// </summary>
        CORRUPT_SYSTEM_FILE = 634,

        /// <summary>
        /// {Virtual Memory Minimum Too Low}
        /// </summary> Your system is low on virtual memory. Windows is increasing the size of your virtual memory paging file.
        // During this process, memory requests for some applications may be denied. For more information, see Help.
        //
        COMMITMENT_MINIMUM = 635,

        /// <summary>
        /// A device was removed so enumeration must be restarted.
        /// </summary>
        PNP_RESTART_ENUMERATION = 636,

        /// <summary>
        /// {Fatal System Error}
        /// </summary> The system image %s is not properly signed.
        // The file has been replaced with the signed file.
        // The system has been shut down.
        //
        SYSTEM_IMAGE_BAD_SIGNATURE = 637,

        /// <summary>
        /// Device will not start without a reboot.
        /// </summary>
        PNP_REBOOT_REQUIRED = 638,

        /// <summary>
        /// There is not enough power to complete the requested operation.
        /// </summary>
        INSUFFICIENT_POWER = 639,

        /// <summary>
        ///  MULTIPLE_FAULT_VIOLATION
        /// </summary>
        MULTIPLE_FAULT_VIOLATION = 640,

        /// <summary>
        /// The system is in the process of shutting down.
        /// </summary>
        SYSTEM_SHUTDOWN = 641,

        /// <summary>
        /// An attempt to remove a processes DebugPort was made, but a port was not already associated with the process.
        /// </summary>
        PORT_NOT_SET = 642,

        /// <summary>
        /// This version of Windows is not compatible with the behavior version of directory forest, domain or domain controller.
        /// </summary>
        DS_VERSION_CHECK_FAILURE = 643,

        /// <summary>
        /// The specified range could not be found in the range list.
        /// </summary>
        RANGE_NOT_FOUND = 644,

        /// <summary>
        /// The driver was not loaded because the system is booting into safe mode.
        /// </summary>
        NOT_SAFE_MODE_DRIVER = 646,

        /// <summary>
        /// The driver was not loaded because it failed its initialization call.
        /// </summary>
        FAILED_DRIVER_ENTRY = 647,

        /// <summary>
        /// The "%hs" encountered an error while applying power or reading the device configuration.
        /// </summary> This may be caused by a failure of your hardware or by a poor connection.
        //
        DEVICE_ENUMERATION_ERROR = 648,

        /// <summary>
        /// The create operation failed because the name contained at least one mount point which resolves to a volume to which the specified device object is not attached.
        /// </summary>
        MOUNT_POINT_NOT_RESOLVED = 649,

        /// <summary>
        /// The device object parameter is either not a valid device object or is not attached to the volume specified by the file name.
        /// </summary>
        INVALID_DEVICE_OBJECT_PARAMETER = 650,

        /// <summary>
        /// A Machine Check Error has occurred. Please check the system eventlog for additional information.
        /// </summary>
        MCA_OCCURED = 651,

        /// <summary>
        /// There was error [%2] processing the driver database.
        /// </summary>
        DRIVER_DATABASE_ERROR = 652,

        /// <summary>
        /// System hive size has exceeded its limit.
        /// </summary>
        SYSTEM_HIVE_TOO_LARGE = 653,

        /// <summary>
        /// The driver could not be loaded because a previous version of the driver is still in memory.
        /// </summary>
        DRIVER_FAILED_PRIOR_UNLOAD = 654,

        /// <summary>
        /// {Volume Shadow Copy Service}
        /// </summary> Please wait while the Volume Shadow Copy Service prepares volume %hs for hibernation.
        //
        VOLSNAP_PREPARE_HIBERNATE = 655,

        /// <summary>
        /// The system has failed to hibernate (The error code is %hs). Hibernation will be disabled until the system is restarted.
        /// </summary>
        HIBERNATION_FAILURE = 656,

        /// <summary>
        /// The password provided is too long to meet the policy of your user account.
        /// </summary> Please choose a shorter password.
        //
        PWD_TOO_LONG = 657,

        /// <summary>
        /// The requested operation could not be completed due to a file system limitation
        /// </summary>
        FILE_SYSTEM_LIMITATION = 665,

        /// <summary>
        /// An assertion failure has occurred.
        /// </summary>
        ASSERTION_FAILURE = 668,

        /// <summary>
        /// An error occurred in the ACPI subsystem.
        /// </summary>
        ACPI_ERROR = 669,

        /// <summary>
        /// WOW Assertion Error.
        /// </summary>
        WOW_ASSERTION = 670,

        /// <summary>
        /// A device is missing in the system BIOS MPS table. This device will not be used.
        /// </summary> Please contact your system vendor for system BIOS update.
        //
        PNP_BAD_MPS_TABLE = 671,

        /// <summary>
        /// A translator failed to translate resources.
        /// </summary>
        PNP_TRANSLATION_FAILED = 672,

        /// <summary>
        /// A IRQ translator failed to translate resources.
        /// </summary>
        PNP_IRQ_TRANSLATION_FAILED = 673,

        /// <summary>
        /// Driver %2 returned invalid ID for a child device (%3).
        /// </summary>
        PNP_INVALID_ID = 674,

        /// <summary>
        /// {Kernel Debugger Awakened}
        /// </summary> the system debugger was awakened by an interrupt.
        //
        WAKE_SYSTEM_DEBUGGER = 675,

        /// <summary>
        /// {Handles Closed}
        /// </summary> Handles to objects have been automatically closed as a result of the requested operation.
        //
        HANDLES_CLOSED = 676,

        /// <summary>
        /// {Too Much Information}
        /// </summary> The specified access control list (ACL) contained more information than was expected.
        //
        EXTRANEOUS_INFORMATION = 677,

        /// <summary>
        /// This warning level status indicates that the transaction state already exists for the registry sub-tree, but that a transaction commit was previously aborted.
        /// </summary> The commit has NOT been completed, but has not been rolled back either (so it may still be committed if desired).
        //
        RXACT_COMMIT_NECESSARY = 678,

        /// <summary>
        /// {Media Changed}
        /// </summary> The media may have changed.
        //
        MEDIA_CHECK = 679,

        /// <summary>
        /// {GUID Substitution}
        /// </summary> During the translation of a global identifier (GUID) to a Windows security ID (SID), no administratively-defined GUID prefix was found.
        // A substitute prefix was used, which will not compromise system security. However, this may provide a more restrictive access than intended.
        //
        GUID_SUBSTITUTION_MADE = 680,

        /// <summary>
        /// The create operation stopped after reaching a symbolic link
        /// </summary>
        STOPPED_ON_SYMLINK = 681,

        /// <summary>
        /// A long jump has been executed.
        /// </summary>
        LONGJUMP = 682,

        /// <summary>
        /// The Plug and Play query operation was not successful.
        /// </summary>
        PLUGPLAY_QUERY_VETOED = 683,

        /// <summary>
        /// A frame consolidation has been executed.
        /// </summary>
        UNWIND_CONSOLIDATE = 684,

        /// <summary>
        /// {Registry Hive Recovered}
        /// </summary> Registry hive (file):
        // %hs
        // was corrupted and it has been recovered. Some data might have been lost.
        //
        REGISTRY_HIVE_RECOVERED = 685,

        /// <summary>
        /// The application is attempting to run executable code from the module %hs. This may be insecure. An alternative, %hs, is available. Should the application use the secure module %hs?
        /// </summary>
        DLL_MIGHT_BE_INSECURE = 686,

        /// <summary>
        /// The application is loading executable code from the module %hs. This is secure, but may be incompatible with previous releases of the operating system. An alternative, %hs, is available. Should the application use the secure module %hs?
        /// </summary>
        DLL_MIGHT_BE_INCOMPATIBLE = 687,

        /// <summary>
        /// Debugger did not handle the exception.
        /// </summary>
        DBG_EXCEPTION_NOT_HANDLED = 688,    // winnt

        /// <summary>
        /// Debugger will reply later.
        /// </summary>
        DBG_REPLY_LATER = 689,

        /// <summary>
        /// Debugger cannot provide handle.
        /// </summary>
        DBG_UNABLE_TO_PROVIDE_HANDLE = 690,

        /// <summary>
        /// Debugger terminated thread.
        /// </summary>
        DBG_TERMINATE_THREAD = 691,    // winnt

        /// <summary>
        /// Debugger terminated process.
        /// </summary>
        DBG_TERMINATE_PROCESS = 692,    // winnt

        /// <summary>
        /// Debugger got control C.
        /// </summary>
        DBG_CONTROL_C = 693,    // winnt

        /// <summary>
        /// Debugger printed exception on control C.
        /// </summary>
        DBG_PRINTEXCEPTION_C = 694,

        /// <summary>
        /// Debugger received RIP exception.
        /// </summary>
        DBG_RIPEXCEPTION = 695,

        /// <summary>
        /// Debugger received control break.
        /// </summary>
        DBG_CONTROL_BREAK = 696,    // winnt

        /// <summary>
        /// Debugger command communication exception.
        /// </summary>
        DBG_COMMAND_EXCEPTION = 697,    // winnt

        /// <summary>
        /// {Object Exists}
        /// </summary> An attempt was made to create an object and the object name already existed.
        //
        OBJECT_NAME_EXISTS = 698,

        /// <summary>
        /// {Thread Suspended}
        /// </summary> A thread termination occurred while the thread was suspended. The thread was resumed, and termination proceeded.
        //
        THREAD_WAS_SUSPENDED = 699,

        /// <summary>
        /// {Image Relocated}
        /// </summary> An image file could not be mapped at the address specified in the image file. Local fixups must be performed on this image.
        //
        IMAGE_NOT_AT_BASE = 700,

        /// <summary>
        /// This informational level status indicates that a specified registry sub-tree transaction state did not yet exist and had to be created.
        /// </summary>
        RXACT_STATE_CREATED = 701,

        /// <summary>
        /// {Segment Load}
        /// </summary> A virtual DOS machine (VDM) is loading, unloading, or moving an MS-DOS or Win16 program segment image.
        // An exception is raised so a debugger can load, unload or track symbols and breakpoints within these 16-bit segments.
        //
        SEGMENT_NOTIFICATION = 702,    // winnt

        /// <summary>
        /// {Invalid Current Directory}
        /// </summary> The process cannot switch to the startup current directory %hs.
        // Select OK to set current directory to %hs, or select CANCEL to exit.
        //
        BAD_CURRENT_DIRECTORY = 703,

        /// <summary>
        /// {Redundant Read}
        /// </summary> To satisfy a read request, the NT fault-tolerant file system successfully read the requested data from a redundant copy.
        // This was done because the file system encountered a failure on a member of the fault-tolerant volume, but was unable to reassign the failing area of the device.
        //
        FT_READ_RECOVERY_FROM_BACKUP = 704,

        /// <summary>
        /// {Redundant Write}
        /// </summary> To satisfy a write request, the NT fault-tolerant file system successfully wrote a redundant copy of the information.
        // This was done because the file system encountered a failure on a member of the fault-tolerant volume, but was not able to reassign the failing area of the device.
        //
        FT_WRITE_RECOVERY = 705,

        /// <summary>
        /// {Machine Type Mismatch}
        /// </summary> The image file %hs is valid, but is for a machine type other than the current machine. Select OK to continue, or CANCEL to fail the DLL load.
        //
        IMAGE_MACHINE_TYPE_MISMATCH = 706,

        /// <summary>
        /// {Partial Data Received}
        /// </summary> The network transport returned partial data to its client. The remaining data will be sent later.
        //
        RECEIVE_PARTIAL = 707,

        /// <summary>
        /// {Expedited Data Received}
        /// </summary> The network transport returned data to its client that was marked as expedited by the remote system.
        //
        RECEIVE_EXPEDITED = 708,

        /// <summary>
        /// {Partial Expedited Data Received}
        /// </summary> The network transport returned partial data to its client and this data was marked as expedited by the remote system. The remaining data will be sent later.
        //
        RECEIVE_PARTIAL_EXPEDITED = 709,

        /// <summary>
        /// {TDI Event Done}
        /// </summary> The TDI indication has completed successfully.
        //
        EVENT_DONE = 710,

        /// <summary>
        /// {TDI Event Pending}
        /// </summary> The TDI indication has entered the pending state.
        //
        EVENT_PENDING = 711,

        /// <summary>
        /// Checking file system on %wZ
        /// </summary>
        CHECKING_FILE_SYSTEM = 712,

        /// <summary>
        /// {Fatal Application Exit}
        /// </summary> %hs
        //
        FATAL_APP_EXIT = 713,

        /// <summary>
        /// The specified registry key is referenced by a predefined handle.
        /// </summary>
        PREDEFINED_HANDLE = 714,

        /// <summary>
        /// {Page Unlocked}
        /// </summary> The page protection of a locked page was changed to 'No Access' and the page was unlocked from memory and from the process.
        //
        WAS_UNLOCKED = 715,

        /// <summary>
        /// %hs
        /// </summary>
        SERVICE_NOTIFICATION = 716,

        /// <summary>
        /// {Page Locked}
        /// </summary> One of the pages to lock was already locked.
        //
        WAS_LOCKED = 717,

        /// <summary>
        /// Application popup: %1 : %2
        /// </summary>
        LOG_HARD_ERROR = 718,

        /// <summary>
        ///  ALREADY_WIN32
        /// </summary>
        ALREADY_WIN32 = 719,

        /// <summary>
        /// {Machine Type Mismatch}
        /// </summary> The image file %hs is valid, but is for a machine type other than the current machine.
        //
        IMAGE_MACHINE_TYPE_MISMATCH_EXE = 720,

        /// <summary>
        /// A yield execution was performed and no thread was available to run.
        /// </summary>
        NO_YIELD_PERFORMED = 721,

        /// <summary>
        /// The resumable flag to a timer API was ignored.
        /// </summary>
        TIMER_RESUME_IGNORED = 722,

        /// <summary>
        /// The arbiter has deferred arbitration of these resources to its parent
        /// </summary>
        ARBITRATION_UNHANDLED = 723,

        /// <summary>
        /// The inserted CardBus device cannot be started because of a configuration error on "%hs".
        /// </summary>
        CARDBUS_NOT_SUPPORTED = 724,

        /// <summary>
        /// The CPUs in this multiprocessor system are not all the same revision level. To use all processors the operating system restricts itself to the features of the least capable processor in the system. Should problems occur with this system, contact the CPU manufacturer to see if this mix of processors is supported.
        /// </summary>
        MP_PROCESSOR_MISMATCH = 725,

        /// <summary>
        /// The system was put into hibernation.
        /// </summary>
        HIBERNATED = 726,

        /// <summary>
        /// The system was resumed from hibernation.
        /// </summary>
        RESUME_HIBERNATION = 727,

        /// <summary>
        /// Windows has detected that the system firmware (BIOS) was updated [previous firmware date = %2, current firmware date %3].
        /// </summary>
        FIRMWARE_UPDATED = 728,

        /// <summary>
        /// A device driver is leaking locked I/O pages causing system degradation. The system has automatically enabled tracking code in order to try and catch the culprit.
        /// </summary>
        DRIVERS_LEAKING_LOCKED_PAGES = 729,

        /// <summary>
        /// The system has awoken
        /// </summary>
        WAKE_SYSTEM = 730,

        /// <summary>
        ///  WAIT_1
        /// </summary>
        WAIT_1 = 731,

        /// <summary>
        ///  WAIT_2
        /// </summary>
        WAIT_2 = 732,

        /// <summary>
        ///  WAIT_3
        /// </summary>
        WAIT_3 = 733,

        /// <summary>
        ///  WAIT_63
        /// </summary>
        WAIT_63 = 734,

        /// <summary>
        ///  ABANDONED_WAIT_0
        /// </summary>
        ABANDONED_WAIT_0 = 735,    // winnt

        /// <summary>
        ///  ABANDONED_WAIT_63
        /// </summary>
        ABANDONED_WAIT_63 = 736,

        /// <summary>
        ///  USER_APC
        /// </summary>
        USER_APC = 737,    // winnt

        /// <summary>
        ///  KERNEL_APC
        /// </summary>
        KERNEL_APC = 738,

        /// <summary>
        ///  ALERTED
        /// </summary>
        ALERTED = 739,

        /// <summary>
        /// The requested operation requires elevation.
        /// </summary>
        ELEVATION_REQUIRED = 740,

        /// <summary>
        /// A reparse should be performed by the Object Manager since the name of the file resulted in a symbolic link.
        /// </summary>
        REPARSE = 741,

        /// <summary>
        /// An open/create operation completed while an oplock break is underway.
        /// </summary>
        OPLOCK_BREAK_IN_PROGRESS = 742,

        /// <summary>
        /// A new volume has been mounted by a file system.
        /// </summary>
        VOLUME_MOUNTED = 743,

        /// <summary>
        /// This success level status indicates that the transaction state already exists for the registry sub-tree, but that a transaction commit was previously aborted.
        /// </summary> The commit has now been completed.
        //
        RXACT_COMMITTED = 744,

        /// <summary>
        /// This indicates that a notify change request has been completed due to closing the handle which made the notify change request.
        /// </summary>
        NOTIFY_CLEANUP = 745,

        /// <summary>
        /// {Connect Failure on Primary Transport}
        /// </summary> An attempt was made to connect to the remote server %hs on the primary transport, but the connection failed.
        // The computer WAS able to connect on a secondary transport.
        //
        PRIMARY_TRANSPORT_CONNECT_FAILED = 746,

        /// <summary>
        /// Page fault was a transition fault.
        /// </summary>
        PAGE_FAULT_TRANSITION = 747,

        /// <summary>
        /// Page fault was a demand zero fault.
        /// </summary>
        PAGE_FAULT_DEMAND_ZERO = 748,

        /// <summary>
        /// Page fault was a demand zero fault.
        /// </summary>
        PAGE_FAULT_COPY_ON_WRITE = 749,

        /// <summary>
        /// Page fault was a demand zero fault.
        /// </summary>
        PAGE_FAULT_GUARD_PAGE = 750,

        /// <summary>
        /// Page fault was satisfied by reading from a secondary storage device.
        /// </summary>
        PAGE_FAULT_PAGING_FILE = 751,

        /// <summary>
        /// Cached page was locked during operation.
        /// </summary>
        CACHE_PAGE_LOCKED = 752,

        /// <summary>
        /// Crash dump exists in paging file.
        /// </summary>
        CRASH_DUMP = 753,

        /// <summary>
        /// Specified buffer contains all zeros.
        /// </summary>
        BUFFER_ALL_ZEROS = 754,

        /// <summary>
        /// A reparse should be performed by the Object Manager since the name of the file resulted in a symbolic link.
        /// </summary>
        REPARSE_OBJECT = 755,

        /// <summary>
        /// The device has succeeded a query-stop and its resource requirements have changed.
        /// </summary>
        RESOURCE_REQUIREMENTS_CHANGED = 756,

        /// <summary>
        /// The translator has translated these resources into the global space and no further translations should be performed.
        /// </summary>
        TRANSLATION_COMPLETE = 757,

        /// <summary>
        /// A process being terminated has no threads to terminate.
        /// </summary>
        NOTHING_TO_TERMINATE = 758,

        /// <summary>
        /// The specified process is not part of a job.
        /// </summary>
        PROCESS_NOT_IN_JOB = 759,

        /// <summary>
        /// The specified process is part of a job.
        /// </summary>
        PROCESS_IN_JOB = 760,

        /// <summary>
        /// {Volume Shadow Copy Service}
        /// </summary> The system is now ready for hibernation.
        //
        VOLSNAP_HIBERNATE_READY = 761,

        /// <summary>
        /// A file system or file system filter driver has successfully completed an FsFilter operation.
        /// </summary>
        FSFILTER_OP_COMPLETED_SUCCESSFULLY = 762,

        /// <summary>
        /// The specified interrupt vector was already connected.
        /// </summary>
        INTERRUPT_VECTOR_ALREADY_CONNECTED = 763,

        /// <summary>
        /// The specified interrupt vector is still connected.
        /// </summary>
        INTERRUPT_STILL_CONNECTED = 764,

        /// <summary>
        /// An operation is blocked waiting for an oplock.
        /// </summary>
        WAIT_FOR_OPLOCK = 765,

        /// <summary>
        /// Debugger handled exception
        /// </summary>
        DBG_EXCEPTION_HANDLED = 766,    // winnt

        /// <summary>
        /// Debugger continued
        /// </summary>
        DBG_CONTINUE = 767,    // winnt

        /// <summary>
        /// An exception occurred in a user mode callback and the kernel callback frame should be removed.
        /// </summary>
        CALLBACK_POP_STACK = 768,

        /// <summary>
        /// Compression is disabled for this volume.
        /// </summary>
        COMPRESSION_DISABLED = 769,

        /// <summary>
        /// The data provider cannot fetch backwards through a result set.
        /// </summary>
        CANTFETCHBACKWARDS = 770,

        /// <summary>
        /// The data provider cannot scroll backwards through a result set.
        /// </summary>
        CANTSCROLLBACKWARDS = 771,

        /// <summary>
        /// The data provider requires that previously fetched data is released before asking for more data.
        /// </summary>
        ROWSNOTRELEASED = 772,

        /// <summary>
        /// The data provider was not able to interpret the flags set for a column binding in an accessor.
        /// </summary>
        BAD_ACCESSOR_FLAGS = 773,

        /// <summary>
        /// One or more errors occurred while processing the request.
        /// </summary>
        ERRORS_ENCOUNTERED = 774,

        /// <summary>
        /// The implementation is not capable of performing the request.
        /// </summary>
        NOT_CAPABLE = 775,

        /// <summary>
        /// The client of a component requested an operation which is not valid given the state of the component instance.
        /// </summary>
        REQUEST_OUT_OF_SEQUENCE = 776,

        /// <summary>
        /// A version number could not be parsed.
        /// </summary>
        VERSION_PARSE_ERROR = 777,

        /// <summary>
        /// The iterator's start position is invalid.
        /// </summary>
        BADSTARTPOSITION = 778,

        /// <summary>
        /// The hardware has reported an uncorrectable memory error.
        /// </summary>
        MEMORY_HARDWARE = 779,

        /// <summary>
        /// The attempted operation required self healing to be enabled.
        /// </summary>
        DISK_REPAIR_DISABLED = 780,

        /// <summary>
        /// The Desktop heap encountered an error while allocating session memory. There is more information in the system event log.
        /// </summary>
        INSUFFICIENT_RESOURCE_FOR_SPECIFIED_SHARED_SECTION_SIZE = 781,

        /// <summary>
        /// The system power state is transitioning from %2 to %3.
        /// </summary>
        SYSTEM_POWERSTATE_TRANSITION = 782,

        /// <summary>
        /// The system power state is transitioning from %2 to %3 but could enter %4.
        /// </summary>
        SYSTEM_POWERSTATE_COMPLEX_TRANSITION = 783,

        /// <summary>
        /// A thread is getting dispatched with MCA EXCEPTION because of MCA.
        /// </summary>
        MCA_EXCEPTION = 784,

        /// <summary>
        /// Access to %1 is monitored by policy rule %2.
        /// </summary>
        ACCESS_AUDIT_BY_POLICY = 785,

        /// <summary>
        /// Access to %1 has been restricted by your Administrator by policy rule %2.
        /// </summary>
        ACCESS_DISABLED_NO_SAFER_UI_BY_POLICY = 786,

        /// <summary>
        /// A valid hibernation file has been invalidated and should be abandoned.
        /// </summary>
        ABANDON_HIBERFILE = 787,

        /// <summary>
        /// {Delayed Write Failed}
        /// </summary> Windows was unable to save all the data for the file %hs; the data has been lost.
        // This error may be caused by network connectivity issues. Please try to save this file elsewhere.
        //
        LOST_WRITEBEHIND_DATA_NETWORK_DISCONNECTED = 788,

        /// <summary>
        /// {Delayed Write Failed}
        /// </summary> Windows was unable to save all the data for the file %hs; the data has been lost.
        // This error was returned by the server on which the file exists. Please try to save this file elsewhere.
        //
        LOST_WRITEBEHIND_DATA_NETWORK_SERVER_ERROR = 789,

        /// <summary>
        /// {Delayed Write Failed}
        /// </summary> Windows was unable to save all the data for the file %hs; the data has been lost.
        // This error may be caused if the device has been removed or the media is write-protected.
        //
        LOST_WRITEBEHIND_DATA_LOCAL_DISK_ERROR = 790,

        /// <summary>
        /// The resources required for this device conflict with the MCFG table.
        /// </summary>
        BAD_MCFG_TABLE = 791,

        /// <summary>
        /// The volume repair could not be performed while it is online.
        /// </summary> Please schedule to take the volume offline so that it can be repaired.
        //
        DISK_REPAIR_REDIRECTED = 792,

        /// <summary>
        /// The volume repair was not successful.
        /// </summary>
        DISK_REPAIR_UNSUCCESSFUL = 793,

        /// <summary>
        /// One of the volume corruption logs is full. Further corruptions that may be detected won't be logged.
        /// </summary>
        CORRUPT_LOG_OVERFULL = 794,

        /// <summary>
        /// One of the volume corruption logs is internally corrupted and needs to be recreated. The volume may contain undetected corruptions and must be scanned.
        /// </summary>
        CORRUPT_LOG_CORRUPTED = 795,

        /// <summary>
        /// One of the volume corruption logs is unavailable for being operated on.
        /// </summary>
        CORRUPT_LOG_UNAVAILABLE = 796,

        /// <summary>
        /// One of the volume corruption logs was deleted while still having corruption records in them. The volume contains detected corruptions and must be scanned.
        /// </summary>
        CORRUPT_LOG_DELETED_FULL = 797,

        /// <summary>
        /// One of the volume corruption logs was cleared by chkdsk and no longer contains real corruptions.
        /// </summary>
        CORRUPT_LOG_CLEARED = 798,

        /// <summary>
        /// Orphaned files exist on the volume but could not be recovered because no more new names could be created in the recovery directory. Files must be moved from the recovery directory.
        /// </summary>
        ORPHAN_NAME_EXHAUSTED = 799,

        /// <summary>
        /// The oplock that was associated with this handle is now associated with a different handle.
        /// </summary>
        OPLOCK_SWITCHED_TO_NEW_HANDLE = 800,

        /// <summary>
        /// An oplock of the requested level cannot be granted.  An oplock of a lower level may be available.
        /// </summary>
        CANNOT_GRANT_REQUESTED_OPLOCK = 801,

        /// <summary>
        /// The operation did not complete successfully because it would cause an oplock to be broken. The caller has requested that existing oplocks not be broken.
        /// </summary>
        CANNOT_BREAK_OPLOCK = 802,

        /// <summary>
        /// The handle with which this oplock was associated has been closed.  The oplock is now broken.
        /// </summary>
        OPLOCK_HANDLE_CLOSED = 803,

        /// <summary>
        /// The specified access control entry (ACE) does not contain a condition.
        /// </summary>
        NO_ACE_CONDITION = 804,

        /// <summary>
        /// The specified access control entry (ACE) contains an invalid condition.
        /// </summary>
        INVALID_ACE_CONDITION = 805,

        /// <summary>
        /// Access to the specified file handle has been revoked.
        /// </summary>
        FILE_HANDLE_REVOKED = 806,

        /// <summary>
        /// {Image Relocated}
        /// </summary> An image file was mapped at a different address from the one specified in the image file but fixups will still be automatically performed on the image.
        //
        IMAGE_AT_DIFFERENT_BASE = 807,

        /// <summary>
        /// **** Available SYSTEM error codes ****
        /// </summary>
        /// <summary>
        /// Access to the extended attribute was denied.
        /// </summary>
        EA_ACCESS_DENIED = 994,

        /// <summary>
        /// The I/O operation has been aborted because of either a thread exit or an application request.
        /// </summary>
        OPERATION_ABORTED = 995,

        /// <summary>
        /// Overlapped I/O event is not in a signaled state.
        /// </summary>
        IO_INCOMPLETE = 996,

        /// <summary>
        /// Overlapped I/O operation is in progress.
        /// </summary>
        IO_PENDING = 997,    // dderror

        /// <summary>
        /// Invalid access to memory location.
        /// </summary>
        NOACCESS = 998,

        /// <summary>
        /// Error performing inpage operation.
        /// </summary>
        SWAPERROR = 999,

        /// <summary>
        /// Recursion too deep; the stack overflowed.
        /// </summary>
        STACK_OVERFLOW = 1001,

        /// <summary>
        /// The window cannot act on the sent message.
        /// </summary>
        INVALID_MESSAGE = 1002,

        /// <summary>
        /// Cannot complete this function.
        /// </summary>
        CAN_NOT_COMPLETE = 1003,

        /// <summary>
        /// Invalid flags.
        /// </summary>
        INVALID_FLAGS = 1004,

        /// <summary>
        /// The volume does not contain a recognized file system.
        /// </summary> Please make sure that all required file system drivers are loaded and that the volume is not corrupted.
        //
        UNRECOGNIZED_VOLUME = 1005,

        /// <summary>
        /// The volume for a file has been externally altered so that the opened file is no longer valid.
        /// </summary>
        FILE_INVALID = 1006,

        /// <summary>
        /// The requested operation cannot be performed in full-screen mode.
        /// </summary>
        FULLSCREEN_MODE = 1007,

        /// <summary>
        /// An attempt was made to reference a token that does not exist.
        /// </summary>
        NO_TOKEN = 1008,

        /// <summary>
        /// The configuration registry database is corrupt.
        /// </summary>
        BADDB = 1009,

        /// <summary>
        /// The configuration registry key is invalid.
        /// </summary>
        BADKEY = 1010,

        /// <summary>
        /// The configuration registry key could not be opened.
        /// </summary>
        CANTOPEN = 1011,

        /// <summary>
        /// The configuration registry key could not be read.
        /// </summary>
        CANTREAD = 1012,

        /// <summary>
        /// The configuration registry key could not be written.
        /// </summary>
        CANTWRITE = 1013,

        /// <summary>
        /// One of the files in the registry database had to be recovered by use of a log or alternate copy. The recovery was successful.
        /// </summary>
        REGISTRY_RECOVERED = 1014,

        /// <summary>
        /// The registry is corrupted. The structure of one of the files containing registry data is corrupted, or the system's memory image of the file is corrupted, or the file could not be recovered because the alternate copy or log was absent or corrupted.
        /// </summary>
        REGISTRY_CORRUPT = 1015,

        /// <summary>
        /// An I/O operation initiated by the registry failed unrecoverably. The registry could not read in, or write out, or flush, one of the files that contain the system's image of the registry.
        /// </summary>
        REGISTRY_IO_FAILED = 1016,

        /// <summary>
        /// The system has attempted to load or restore a file into the registry, but the specified file is not in a registry file format.
        /// </summary>
        NOT_REGISTRY_FILE = 1017,

        /// <summary>
        /// Illegal operation attempted on a registry key that has been marked for deletion.
        /// </summary>
        KEY_DELETED = 1018,

        /// <summary>
        /// System could not allocate the required space in a registry log.
        /// </summary>
        NO_LOG_SPACE = 1019,

        /// <summary>
        /// Cannot create a symbolic link in a registry key that already has subkeys or values.
        /// </summary>
        KEY_HAS_CHILDREN = 1020,

        /// <summary>
        /// Cannot create a stable subkey under a volatile parent key.
        /// </summary>
        CHILD_MUST_BE_VOLATILE = 1021,

        /// <summary>
        /// A notify change request is being completed and the information is not being returned in the caller's buffer. The caller now needs to enumerate the files to find the changes.
        /// </summary>
        NOTIFY_ENUM_DIR = 1022,

        /// <summary>
        /// A stop control has been sent to a service that other running services are dependent on.
        /// </summary>
        DEPENDENT_SERVICES_RUNNING = 1051,

        /// <summary>
        /// The requested control is not valid for this service.
        /// </summary>
        INVALID_SERVICE_CONTROL = 1052,

        /// <summary>
        /// The service did not respond to the start or control request in a timely fashion.
        /// </summary>
        SERVICE_REQUEST_TIMEOUT = 1053,

        /// <summary>
        /// A thread could not be created for the service.
        /// </summary>
        SERVICE_NO_THREAD = 1054,

        /// <summary>
        /// The service database is locked.
        /// </summary>
        SERVICE_DATABASE_LOCKED = 1055,

        /// <summary>
        /// An instance of the service is already running.
        /// </summary>
        SERVICE_ALREADY_RUNNING = 1056,

        /// <summary>
        /// The account name is invalid or does not exist, or the password is invalid for the account name specified.
        /// </summary>
        INVALID_SERVICE_ACCOUNT = 1057,

        /// <summary>
        /// The service cannot be started, either because it is disabled or because it has no enabled devices associated with it.
        /// </summary>
        SERVICE_DISABLED = 1058,

        /// <summary>
        /// Circular service dependency was specified.
        /// </summary>
        CIRCULAR_DEPENDENCY = 1059,

        /// <summary>
        /// The specified service does not exist as an installed service.
        /// </summary>
        SERVICE_DOES_NOT_EXIST = 1060,

        /// <summary>
        /// The service cannot accept control messages at this time.
        /// </summary>
        SERVICE_CANNOT_ACCEPT_CTRL = 1061,

        /// <summary>
        /// The service has not been started.
        /// </summary>
        SERVICE_NOT_ACTIVE = 1062,

        /// <summary>
        /// The service process could not connect to the service controller.
        /// </summary>
        FAILED_SERVICE_CONTROLLER_CONNECT = 1063,

        /// <summary>
        /// An exception occurred in the service when handling the control request.
        /// </summary>
        EXCEPTION_IN_SERVICE = 1064,

        /// <summary>
        /// The database specified does not exist.
        /// </summary>
        DATABASE_DOES_NOT_EXIST = 1065,

        /// <summary>
        /// The service has returned a service-specific error code.
        /// </summary>
        SERVICE_SPECIFIC_ERROR = 1066,

        /// <summary>
        /// The process terminated unexpectedly.
        /// </summary>
        PROCESS_ABORTED = 1067,

        /// <summary>
        /// The dependency service or group failed to start.
        /// </summary>
        SERVICE_DEPENDENCY_FAIL = 1068,

        /// <summary>
        /// The service did not start due to a logon failure.
        /// </summary>
        SERVICE_LOGON_FAILED = 1069,

        /// <summary>
        /// After starting, the service hung in a start-pending state.
        /// </summary>
        SERVICE_START_HANG = 1070,

        /// <summary>
        /// The specified service database lock is invalid.
        /// </summary>
        INVALID_SERVICE_LOCK = 1071,

        /// <summary>
        /// The specified service has been marked for deletion.
        /// </summary>
        SERVICE_MARKED_FOR_DELETE = 1072,

        /// <summary>
        /// The specified service already exists.
        /// </summary>
        SERVICE_EXISTS = 1073,

        /// <summary>
        /// The system is currently running with the last-known-good configuration.
        /// </summary>
        ALREADY_RUNNING_LKG = 1074,

        /// <summary>
        /// The dependency service does not exist or has been marked for deletion.
        /// </summary>
        SERVICE_DEPENDENCY_DELETED = 1075,

        /// <summary>
        /// The current boot has already been accepted for use as the last-known-good control set.
        /// </summary>
        BOOT_ALREADY_ACCEPTED = 1076,

        /// <summary>
        /// No attempts to start the service have been made since the last boot.
        /// </summary>
        SERVICE_NEVER_STARTED = 1077,

        /// <summary>
        /// The name is already in use as either a service name or a service display name.
        /// </summary>
        DUPLICATE_SERVICE_NAME = 1078,

        /// <summary>
        /// The account specified for this service is different from the account specified for other services running in the same process.
        /// </summary>
        DIFFERENT_SERVICE_ACCOUNT = 1079,

        /// <summary>
        /// Failure actions can only be set for Win32 services, not for drivers.
        /// </summary>
        CANNOT_DETECT_DRIVER_FAILURE = 1080,

        /// <summary>
        /// This service runs in the same process as the service control manager.
        /// </summary> Therefore, the service control manager cannot take action if this service's process terminates unexpectedly.
        //
        CANNOT_DETECT_PROCESS_ABORT = 1081,

        /// <summary>
        /// No recovery program has been configured for this service.
        /// </summary>
        NO_RECOVERY_PROGRAM = 1082,

        /// <summary>
        /// The executable program that this service is configured to run in does not implement the service.
        /// </summary>
        SERVICE_NOT_IN_EXE = 1083,

        /// <summary>
        /// This service cannot be started in Safe Mode
        /// </summary>
        NOT_SAFEBOOT_SERVICE = 1084,

        /// <summary>
        /// The physical end of the tape has been reached.
        /// </summary>
        END_OF_MEDIA = 1100,

        /// <summary>
        /// A tape access reached a filemark.
        /// </summary>
        FILEMARK_DETECTED = 1101,

        /// <summary>
        /// The beginning of the tape or a partition was encountered.
        /// </summary>
        BEGINNING_OF_MEDIA = 1102,

        /// <summary>
        /// A tape access reached the end of a set of files.
        /// </summary>
        SETMARK_DETECTED = 1103,

        /// <summary>
        /// No more data is on the tape.
        /// </summary>
        NO_DATA_DETECTED = 1104,

        /// <summary>
        /// Tape could not be partitioned.
        /// </summary>
        PARTITION_FAILURE = 1105,

        /// <summary>
        /// When accessing a new tape of a multivolume partition, the current block size is incorrect.
        /// </summary>
        INVALID_BLOCK_LENGTH = 1106,

        /// <summary>
        /// Tape partition information could not be found when loading a tape.
        /// </summary>
        DEVICE_NOT_PARTITIONED = 1107,

        /// <summary>
        /// Unable to lock the media eject mechanism.
        /// </summary>
        UNABLE_TO_LOCK_MEDIA = 1108,

        /// <summary>
        /// Unable to unload the media.
        /// </summary>
        UNABLE_TO_UNLOAD_MEDIA = 1109,

        /// <summary>
        /// The media in the drive may have changed.
        /// </summary>
        MEDIA_CHANGED = 1110,

        /// <summary>
        /// The I/O bus was reset.
        /// </summary>
        BUS_RESET = 1111,

        /// <summary>
        /// No media in drive.
        /// </summary>
        NO_MEDIA_IN_DRIVE = 1112,

        /// <summary>
        /// No mapping for the Unicode character exists in the target multi-byte code page.
        /// </summary>
        NO_UNICODE_TRANSLATION = 1113,

        /// <summary>
        /// A dynamic link library (DLL) initialization routine failed.
        /// </summary>
        DLL_INIT_FAILED = 1114,

        /// <summary>
        /// A system shutdown is in progress.
        /// </summary>
        SHUTDOWN_IN_PROGRESS = 1115,

        /// <summary>
        /// Unable to abort the system shutdown because no shutdown was in progress.
        /// </summary>
        NO_SHUTDOWN_IN_PROGRESS = 1116,

        /// <summary>
        /// The request could not be performed because of an I/O device error.
        /// </summary>
        IO_DEVICE = 1117,

        /// <summary>
        /// No serial device was successfully initialized. The serial driver will unload.
        /// </summary>
        SERIAL_NO_DEVICE = 1118,

        /// <summary>
        /// Unable to open a device that was sharing an interrupt request (IRQ) with other devices. At least one other device that uses that IRQ was already opened.
        /// </summary>
        IRQ_BUSY = 1119,

        /// <summary>
        /// A serial I/O operation was completed by another write to the serial port.
        /// </summary> (The IOCTL_SERIAL_XOFF_COUNTER reached zero.)
        //
        MORE_WRITES = 1120,

        /// <summary>
        /// A serial I/O operation completed because the timeout period expired.
        /// </summary> (The IOCTL_SERIAL_XOFF_COUNTER did not reach zero.)
        //
        COUNTER_TIMEOUT = 1121,

        /// <summary>
        /// No ID address mark was found on the floppy disk.
        /// </summary>
        FLOPPY_ID_MARK_NOT_FOUND = 1122,

        /// <summary>
        /// Mismatch between the floppy disk sector ID field and the floppy disk controller track address.
        /// </summary>
        FLOPPY_WRONG_CYLINDER = 1123,

        /// <summary>
        /// The floppy disk controller reported an error that is not recognized by the floppy disk driver.
        /// </summary>
        FLOPPY_UNKNOWN_ERROR = 1124,

        /// <summary>
        /// The floppy disk controller returned inconsistent results in its registers.
        /// </summary>
        FLOPPY_BAD_REGISTERS = 1125,

        /// <summary>
        /// While accessing the hard disk, a recalibrate operation failed, even after retries.
        /// </summary>
        DISK_RECALIBRATE_FAILED = 1126,

        /// <summary>
        /// While accessing the hard disk, a disk operation failed even after retries.
        /// </summary>
        DISK_OPERATION_FAILED = 1127,

        /// <summary>
        /// While accessing the hard disk, a disk controller reset was needed, but even that failed.
        /// </summary>
        DISK_RESET_FAILED = 1128,

        /// <summary>
        /// Physical end of tape encountered.
        /// </summary>
        EOM_OVERFLOW = 1129,

        /// <summary>
        /// Not enough server storage is available to process this command.
        /// </summary>
        NOT_ENOUGH_SERVER_MEMORY = 1130,

        /// <summary>
        /// A potential deadlock condition has been detected.
        /// </summary>
        POSSIBLE_DEADLOCK = 1131,

        /// <summary>
        /// The base address or the file offset specified does not have the proper alignment.
        /// </summary>
        MAPPED_ALIGNMENT = 1132,

        /// <summary>
        /// An attempt to change the system power state was vetoed by another application or driver.
        /// </summary>
        SET_POWER_STATE_VETOED = 1140,

        /// <summary>
        /// The system BIOS failed an attempt to change the system power state.
        /// </summary>
        SET_POWER_STATE_FAILED = 1141,

        /// <summary>
        /// An attempt was made to create more links on a file than the file system supports.
        /// </summary>
        TOO_MANY_LINKS = 1142,

        /// <summary>
        /// The specified program requires a newer version of Windows.
        /// </summary>
        OLD_WIN_VERSION = 1150,

        /// <summary>
        /// The specified program is not a Windows or MS-DOS program.
        /// </summary>
        APP_WRONG_OS = 1151,

        /// <summary>
        /// Cannot start more than one instance of the specified program.
        /// </summary>
        SINGLE_INSTANCE_APP = 1152,

        /// <summary>
        /// The specified program was written for an earlier version of Windows.
        /// </summary>
        RMODE_APP = 1153,

        /// <summary>
        /// One of the library files needed to run this application is damaged.
        /// </summary>
        INVALID_DLL = 1154,

        /// <summary>
        /// No application is associated with the specified file for this operation.
        /// </summary>
        NO_ASSOCIATION = 1155,

        /// <summary>
        /// An error occurred in sending the command to the application.
        /// </summary>
        DDE_FAIL = 1156,

        /// <summary>
        /// One of the library files needed to run this application cannot be found.
        /// </summary>
        DLL_NOT_FOUND = 1157,

        /// <summary>
        /// The current process has used all of its system allowance of handles for Window Manager objects.
        /// </summary>
        NO_MORE_USER_HANDLES = 1158,

        /// <summary>
        /// The message can be used only with synchronous operations.
        /// </summary>
        MESSAGE_SYNC_ONLY = 1159,

        /// <summary>
        /// The indicated source element has no media.
        /// </summary>
        SOURCE_ELEMENT_EMPTY = 1160,

        /// <summary>
        /// The indicated destination element already contains media.
        /// </summary>
        DESTINATION_ELEMENT_FULL = 1161,

        /// <summary>
        /// The indicated element does not exist.
        /// </summary>
        ILLEGAL_ELEMENT_ADDRESS = 1162,

        /// <summary>
        /// The indicated element is part of a magazine that is not present.
        /// </summary>
        MAGAZINE_NOT_PRESENT = 1163,

        /// <summary>
        /// The indicated device requires reinitialization due to hardware errors.
        /// </summary>
        DEVICE_REINITIALIZATION_NEEDED = 1164,    // dderror

        /// <summary>
        /// The device has indicated that cleaning is required before further operations are attempted.
        /// </summary>
        DEVICE_REQUIRES_CLEANING = 1165,

        /// <summary>
        /// The device has indicated that its door is open.
        /// </summary>
        DEVICE_DOOR_OPEN = 1166,

        /// <summary>
        /// The device is not connected.
        /// </summary>
        DEVICE_NOT_CONNECTED = 1167,

        /// <summary>
        /// Element not found.
        /// </summary>
        NOT_FOUND = 1168,

        /// <summary>
        /// There was no match for the specified key in the index.
        /// </summary>
        NO_MATCH = 1169,

        /// <summary>
        /// The property set specified does not exist on the object.
        /// </summary>
        SET_NOT_FOUND = 1170,

        /// <summary>
        /// The point passed to GetMouseMovePoints is not in the buffer.
        /// </summary>
        POINT_NOT_FOUND = 1171,

        /// <summary>
        /// The tracking (workstation) service is not running.
        /// </summary>
        NO_TRACKING_SERVICE = 1172,

        /// <summary>
        /// The Volume ID could not be found.
        /// </summary>
        NO_VOLUME_ID = 1173,

        /// <summary>
        /// Unable to remove the file to be replaced.
        /// </summary>
        UNABLE_TO_REMOVE_REPLACED = 1175,

        /// <summary>
        /// Unable to move the replacement file to the file to be replaced. The file to be replaced has retained its original name.
        /// </summary>
        UNABLE_TO_MOVE_REPLACEMENT = 1176,

        /// <summary>
        /// Unable to move the replacement file to the file to be replaced. The file to be replaced has been renamed using the backup name.
        /// </summary>
        UNABLE_TO_MOVE_REPLACEMENT_2 = 1177,

        /// <summary>
        /// The volume change journal is being deleted.
        /// </summary>
        JOURNAL_DELETE_IN_PROGRESS = 1178,

        /// <summary>
        /// The volume change journal is not active.
        /// </summary>
        JOURNAL_NOT_ACTIVE = 1179,

        /// <summary>
        /// A file was found, but it may not be the correct file.
        /// </summary>
        POTENTIAL_FILE_FOUND = 1180,

        /// <summary>
        /// The journal entry has been deleted from the journal.
        /// </summary>
        JOURNAL_ENTRY_DELETED = 1181,

        /// <summary>
        /// A system shutdown has already been scheduled.
        /// </summary>
        SHUTDOWN_IS_SCHEDULED = 1190,

        /// <summary>
        /// The system shutdown cannot be initiated because there are other users logged on to the computer.
        /// </summary>
        SHUTDOWN_USERS_LOGGED_ON = 1191,

        /// <summary>
        /// The specified device name is invalid.
        /// </summary>
        BAD_DEVICE = 1200,

        /// <summary>
        /// The device is not currently connected but it is a remembered connection.
        /// </summary>
        CONNECTION_UNAVAIL = 1201,

        /// <summary>
        /// The local device name has a remembered connection to another network resource.
        /// </summary>
        DEVICE_ALREADY_REMEMBERED = 1202,

        /// <summary>
        /// The network path was either typed incorrectly, does not exist, or the network provider is not currently available. Please try retyping the path or contact your network administrator.
        /// </summary>
        NO_NET_OR_BAD_PATH = 1203,

        /// <summary>
        /// The specified network provider name is invalid.
        /// </summary>
        BAD_PROVIDER = 1204,

        /// <summary>
        /// Unable to open the network connection profile.
        /// </summary>
        CANNOT_OPEN_PROFILE = 1205,

        /// <summary>
        /// The network connection profile is corrupted.
        /// </summary>
        BAD_PROFILE = 1206,

        /// <summary>
        /// Cannot enumerate a noncontainer.
        /// </summary>
        NOT_CONTAINER = 1207,

        /// <summary>
        /// An extended error has occurred.
        /// </summary>
        EXTENDED_ERROR = 1208,

        /// <summary>
        /// The format of the specified group name is invalid.
        /// </summary>
        INVALID_GROUPNAME = 1209,

        /// <summary>
        /// The format of the specified computer name is invalid.
        /// </summary>
        INVALID_COMPUTERNAME = 1210,

        /// <summary>
        /// The format of the specified event name is invalid.
        /// </summary>
        INVALID_EVENTNAME = 1211,

        /// <summary>
        /// The format of the specified domain name is invalid.
        /// </summary>
        INVALID_DOMAINNAME = 1212,

        /// <summary>
        /// The format of the specified service name is invalid.
        /// </summary>
        INVALID_SERVICENAME = 1213,

        /// <summary>
        /// The format of the specified network name is invalid.
        /// </summary>
        INVALID_NETNAME = 1214,

        /// <summary>
        /// The format of the specified share name is invalid.
        /// </summary>
        INVALID_SHARENAME = 1215,

        /// <summary>
        /// The format of the specified password is invalid.
        /// </summary>
        INVALID_PASSWORDNAME = 1216,

        /// <summary>
        /// The format of the specified message name is invalid.
        /// </summary>
        INVALID_MESSAGENAME = 1217,

        /// <summary>
        /// The format of the specified message destination is invalid.
        /// </summary>
        INVALID_MESSAGEDEST = 1218,

        /// <summary>
        /// Multiple connections to a server or shared resource by the same user, using more than one user name, are not allowed. Disconnect all previous connections to the server or shared resource and try again.
        /// </summary>
        SESSION_CREDENTIAL_CONFLICT = 1219,

        /// <summary>
        /// An attempt was made to establish a session to a network server, but there are already too many sessions established to that server.
        /// </summary>
        REMOTE_SESSION_LIMIT_EXCEEDED = 1220,

        /// <summary>
        /// The workgroup or domain name is already in use by another computer on the network.
        /// </summary>
        DUP_DOMAINNAME = 1221,

        /// <summary>
        /// The network is not present or not started.
        /// </summary>
        NO_NETWORK = 1222,

        /// <summary>
        /// The operation was canceled by the user.
        /// </summary>
        CANCELLED = 1223,

        /// <summary>
        /// The requested operation cannot be performed on a file with a user-mapped section open.
        /// </summary>
        USER_MAPPED_FILE = 1224,

        /// <summary>
        /// The remote computer refused the network connection.
        /// </summary>
        CONNECTION_REFUSED = 1225,

        /// <summary>
        /// The network connection was gracefully closed.
        /// </summary>
        GRACEFUL_DISCONNECT = 1226,

        /// <summary>
        /// The network transport endpoint already has an address associated with it.
        /// </summary>
        ADDRESS_ALREADY_ASSOCIATED = 1227,

        /// <summary>
        /// An address has not yet been associated with the network endpoint.
        /// </summary>
        ADDRESS_NOT_ASSOCIATED = 1228,

        /// <summary>
        /// An operation was attempted on a nonexistent network connection.
        /// </summary>
        CONNECTION_INVALID = 1229,

        /// <summary>
        /// An invalid operation was attempted on an active network connection.
        /// </summary>
        CONNECTION_ACTIVE = 1230,

        /// <summary>
        /// The network location cannot be reached. For information about network troubleshooting, see Windows Help.
        /// </summary>
        NETWORK_UNREACHABLE = 1231,

        /// <summary>
        /// The network location cannot be reached. For information about network troubleshooting, see Windows Help.
        /// </summary>
        HOST_UNREACHABLE = 1232,

        /// <summary>
        /// The network location cannot be reached. For information about network troubleshooting, see Windows Help.
        /// </summary>
        PROTOCOL_UNREACHABLE = 1233,

        /// <summary>
        /// No service is operating at the destination network endpoint on the remote system.
        /// </summary>
        PORT_UNREACHABLE = 1234,

        /// <summary>
        /// The request was aborted.
        /// </summary>
        REQUEST_ABORTED = 1235,

        /// <summary>
        /// The network connection was aborted by the local system.
        /// </summary>
        CONNECTION_ABORTED = 1236,

        /// <summary>
        /// The operation could not be completed. A retry should be performed.
        /// </summary>
        RETRY = 1237,

        /// <summary>
        /// A connection to the server could not be made because the limit on the number of concurrent connections for this account has been reached.
        /// </summary>
        CONNECTION_COUNT_LIMIT = 1238,

        /// <summary>
        /// Attempting to log in during an unauthorized time of day for this account.
        /// </summary>
        LOGIN_TIME_RESTRICTION = 1239,

        /// <summary>
        /// The account is not authorized to log in from this station.
        /// </summary>
        LOGIN_WKSTA_RESTRICTION = 1240,

        /// <summary>
        /// The network address could not be used for the operation requested.
        /// </summary>
        INCORRECT_ADDRESS = 1241,

        /// <summary>
        /// The service is already registered.
        /// </summary>
        ALREADY_REGISTERED = 1242,

        /// <summary>
        /// The specified service does not exist.
        /// </summary>
        SERVICE_NOT_FOUND = 1243,

        /// <summary>
        /// The operation being requested was not performed because the user has not been authenticated.
        /// </summary>
        NOT_AUTHENTICATED = 1244,

        /// <summary>
        /// The operation being requested was not performed because the user has not logged on to the network. The specified service does not exist.
        /// </summary>
        NOT_LOGGED_ON = 1245,

        /// <summary>
        /// Continue with work in progress.
        /// </summary>
        CONTINUE = 1246,    // dderror

        /// <summary>
        /// An attempt was made to perform an initialization operation when initialization has already been completed.
        /// </summary>
        ALREADY_INITIALIZED = 1247,

        /// <summary>
        /// No more local devices.
        /// </summary>
        NO_MORE_DEVICES = 1248,    // dderror

        /// <summary>
        /// The specified site does not exist.
        /// </summary>
        NO_SUCH_SITE = 1249,

        /// <summary>
        /// A domain controller with the specified name already exists.
        /// </summary>
        DOMAIN_CONTROLLER_EXISTS = 1250,

        /// <summary>
        /// This operation is supported only when you are connected to the server.
        /// </summary>
        ONLY_IF_CONNECTED = 1251,

        /// <summary>
        /// The group policy framework should call the extension even if there are no changes.
        /// </summary>
        OVERRIDE_NOCHANGES = 1252,

        /// <summary>
        /// The specified user does not have a valid profile.
        /// </summary>
        BAD_USER_PROFILE = 1253,

        /// <summary>
        /// This operation is not supported on a computer running Windows Server 2003 for Small Business Server
        /// </summary>
        NOT_SUPPORTED_ON_SBS = 1254,

        /// <summary>
        /// The server machine is shutting down.
        /// </summary>
        SERVER_SHUTDOWN_IN_PROGRESS = 1255,

        /// <summary>
        /// The remote system is not available. For information about network troubleshooting, see Windows Help.
        /// </summary>
        HOST_DOWN = 1256,

        /// <summary>
        /// The security identifier provided is not from an account domain.
        /// </summary>
        NON_ACCOUNT_SID = 1257,

        /// <summary>
        /// The security identifier provided does not have a domain component.
        /// </summary>
        NON_DOMAIN_SID = 1258,

        /// <summary>
        /// AppHelp dialog canceled thus preventing the application from starting.
        /// </summary>
        APPHELP_BLOCK = 1259,

        /// <summary>
        /// This program is blocked by group policy. For more information, contact your system administrator.
        /// </summary>
        ACCESS_DISABLED_BY_POLICY = 1260,

        /// <summary>
        /// A program attempt to use an invalid register value. Normally caused by an uninitialized register. This error is Itanium specific.
        /// </summary>
        REG_NAT_CONSUMPTION = 1261,

        /// <summary>
        /// The share is currently offline or does not exist.
        /// </summary>
        CSCSHARE_OFFLINE = 1262,

        /// <summary>
        /// The Kerberos protocol encountered an error while validating the KDC certificate during smartcard logon. There is more information in the system event log.
        /// </summary>
        PKINIT_FAILURE = 1263,

        /// <summary>
        /// The Kerberos protocol encountered an error while attempting to utilize the smartcard subsystem.
        /// </summary>
        SMARTCARD_SUBSYSTEM_FAILURE = 1264,

        /// <summary>
        /// The system cannot contact a domain controller to service the authentication request. Please try again later.
        /// </summary>
        DOWNGRADE_DETECTED = 1265,

        /// <summary>
        /// Do not use ID's 1266 - 1270 as the symbolicNames have been moved to SEC_E_*
        /// </summary>
        /// <summary>
        /// The machine is locked and cannot be shut down without the force option.
        /// </summary>
        MACHINE_LOCKED = 1271,

        /// <summary>
        /// An application-defined callback gave invalid data when called.
        /// </summary>
        CALLBACK_SUPPLIED_INVALID_DATA = 1273,

        /// <summary>
        /// The group policy framework should call the extension in the synchronous foreground policy refresh.
        /// </summary>
        SYNC_FOREGROUND_REFRESH_REQUIRED = 1274,

        /// <summary>
        /// This driver has been blocked from loading
        /// </summary>
        DRIVER_BLOCKED = 1275,

        /// <summary>
        /// A dynamic link library (DLL) referenced a module that was neither a DLL nor the process's executable image.
        /// </summary>
        INVALID_IMPORT_OF_NON_DLL = 1276,

        /// <summary>
        /// Windows cannot open this program since it has been disabled.
        /// </summary>
        ACCESS_DISABLED_WEBBLADE = 1277,

        /// <summary>
        /// Windows cannot open this program because the license enforcement system has been tampered with or become corrupted.
        /// </summary>
        ACCESS_DISABLED_WEBBLADE_TAMPER = 1278,

        /// <summary>
        /// A transaction recover failed.
        /// </summary>
        RECOVERY_FAILURE = 1279,

        /// <summary>
        /// The current thread has already been converted to a fiber.
        /// </summary>
        ALREADY_FIBER = 1280,

        /// <summary>
        /// The current thread has already been converted from a fiber.
        /// </summary>
        ALREADY_THREAD = 1281,

        /// <summary>
        /// The system detected an overrun of a stack-based buffer in this application. This overrun could potentially allow a malicious user to gain control of this application.
        /// </summary>
        STACK_BUFFER_OVERRUN = 1282,

        /// <summary>
        /// Data present in one of the parameters is more than the function can operate on.
        /// </summary>
        PARAMETER_QUOTA_EXCEEDED = 1283,

        /// <summary>
        /// An attempt to do an operation on a debug object failed because the object is in the process of being deleted.
        /// </summary>
        DEBUGGER_INACTIVE = 1284,

        /// <summary>
        /// An attempt to delay-load a .dll or get a function address in a delay-loaded .dll failed.
        /// </summary>
        DELAY_LOAD_FAILED = 1285,

        /// <summary>
        /// %1 is a 16-bit application. You do not have permissions to execute 16-bit applications. Check your permissions with your system administrator.
        /// </summary>
        VDM_DISALLOWED = 1286,

        /// <summary>
        /// Insufficient information exists to identify the cause of failure.
        /// </summary>
        UNIDENTIFIED_ERROR = 1287,

        /// <summary>
        /// The parameter passed to a C runtime function is incorrect.
        /// </summary>
        INVALID_CRUNTIME_PARAMETER = 1288,

        /// <summary>
        /// The operation occurred beyond the valid data length of the file.
        /// </summary>
        BEYOND_VDL = 1289,

        /// <summary>
        /// The service start failed since one or more services in the same process have an incompatible service SID type setting. A service with restricted service SID type can only coexist in the same process with other services with a restricted SID type. If the service SID type for this service was just configured, the hosting process must be restarted in order to start this service.
        /// </summary>
        INCOMPATIBLE_SERVICE_SID_TYPE = 1290,

        /// <summary>
        /// The process hosting the driver for this device has been terminated.
        /// </summary>
        DRIVER_PROCESS_TERMINATED = 1291,

        /// <summary>
        /// An operation attempted to exceed an implementation-defined limit.
        /// </summary>
        IMPLEMENTATION_LIMIT = 1292,

        /// <summary>
        /// Either the target process, or the target thread's containing process, is a protected process.
        /// </summary>
        PROCESS_IS_PROTECTED = 1293,

        /// <summary>
        /// The service notification client is lagging too far behind the current state of services in the machine.
        /// </summary>
        SERVICE_NOTIFY_CLIENT_LAGGING = 1294,

        /// <summary>
        /// The requested file operation failed because the storage quota was exceeded.
        /// </summary> To free up disk space, move files to a different location or delete unnecessary files. For more information, contact your system administrator.
        //
        DISK_QUOTA_EXCEEDED = 1295,

        /// <summary>
        /// The requested file operation failed because the storage policy blocks that type of file. For more information, contact your system administrator.
        /// </summary>
        CONTENT_BLOCKED = 1296,

        /// <summary>
        /// A privilege that the service requires to function properly does not exist in the service account configuration.
        /// </summary> You may use the Services Microsoft Management Console (MMC) snap-in (services.msc) and the Local Security Settings MMC snap-in (secpol.msc) to view the service configuration and the account configuration.
        //
        INCOMPATIBLE_SERVICE_PRIVILEGE = 1297,

        /// <summary>
        /// A thread involved in this operation appears to be unresponsive.
        /// </summary>
        APP_HANG = 1298,


    }
}
