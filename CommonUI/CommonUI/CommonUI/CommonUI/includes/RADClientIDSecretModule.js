var RADClientIDSecretModule;RADClientIDSecretModule =
/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./components/IdList.js":
/*!******************************!*\
  !*** ./components/IdList.js ***!
  \******************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => /* binding */ RADClientIDList
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);

function RADClientIDList(props) {
  function maskstring(text) {
    var newtext = text.slice(0, 4) + '*'.repeat(text.length - 8) + text.slice(-4);
    return newtext;
  }

  function cpyToClipboard(value) {
    var tempInput = document.createElement("input");
    tempInput.value = value;
    document.body.appendChild(tempInput);
    tempInput.select();
    document.execCommand("copy");
    document.body.removeChild(tempInput);
  }

  var style = {
    width: 'calc(100% - 20px)',
    "overflow": "hidden",
    "text-overflow": "ellipsis",
    "white-space": "nowrap"
  };
  return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDListMain"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDListHeader"
  }, "Configured Client IDs ", props.idList.length > 0 && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("span", null, "(", props.idList.length, ")")), props.idList.length > 0 && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDHeader"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDHeaderEach RADClientIDHeaderUserName"
  }, "Username"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDHeaderEach RADClientIDHeaderDesc"
  }, "Description"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDHeaderEach RADClientIDHeaderExpiry"
  }, "Expiry"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDHeaderEach RADClientIDHeaderNotification"
  }, "Notification"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDHeaderEach RADClientIDHeaderClientID"
  }, "Client ID"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDHeaderEach RADClientIDHeaderClientSec"
  }, "Client Secret")), props.idList.map(function (x, i) {
    return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      className: "RADClientIDEach"
    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      className: "RADClientIDEachDetails"
    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      title: x.UserName,
      className: "RADClientIDCell RADClientIDUser"
    }, x.UserName), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      title: x.Description,
      className: "RADClientIDCell RADClientIDDesc"
    }, x.Description), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      title: moment(x.Expiry).format('DD/MM/YYYY'),
      className: "RADClientIDCell RADClientIDExpiry"
    }, moment(x.Expiry).format('DD/MM/YYYY')), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      title: x.NotificationMailIds,
      className: "RADClientIDCell RADClientNotification"
    }, x.NotificationMailIds), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      title: x.NewId ? x.ClientID : "",
      className: "RADClientIDCell RADClientID"
    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      style: style
    }, x.NewId ? x.ClientID : maskstring(x.ClientID)), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("span", {
      onClick: function onClick() {
        return cpyToClipboard(x.ClientID);
      },
      className: "RADCpytoClipIcon"
    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("i", {
      "class": "fas fa-copy"
    }))), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      title: x.NewId ? x.ClientSecret : "",
      className: "RADClientIDCell RADClientSecret"
    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      style: style
    }, x.NewId ? x.ClientSecret : maskstring(x.ClientSecret)), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("span", {
      onClick: function onClick() {
        return cpyToClipboard(x.ClientSecret);
      },
      className: "RADCpytoClipIcon"
    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("i", {
      "class": "fas fa-copy"
    })))), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      onClick: function onClick() {
        return props.RenewClientID(x.id);
      },
      className: "RADClientIDRenew"
    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("i", {
      "class": "fas fa-redo"
    })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      onClick: function onClick() {
        return props.DeleteClientIDSecret(x.id);
      },
      className: "RADClientIDDelete"
    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("i", {
      className: "fas fa-trash-alt"
    })));
  }));
}

/***/ }),

/***/ "./components/app.js":
/*!***************************!*\
  !*** ./components/app.js ***!
  \***************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => /* binding */ RADClientIDSecretApp
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _createID__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./createID */ "./components/createID.js");
/* harmony import */ var _IdList__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./IdList */ "./components/IdList.js");
/* harmony import */ var _createNewUser__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./createNewUser */ "./components/createNewUser.js");
function ownKeys(object, enumerableOnly) { var keys = Object.keys(object); if (Object.getOwnPropertySymbols) { var symbols = Object.getOwnPropertySymbols(object); if (enumerableOnly) symbols = symbols.filter(function (sym) { return Object.getOwnPropertyDescriptor(object, sym).enumerable; }); keys.push.apply(keys, symbols); } return keys; }

function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i] != null ? arguments[i] : {}; if (i % 2) { ownKeys(Object(source), true).forEach(function (key) { _defineProperty(target, key, source[key]); }); } else if (Object.getOwnPropertyDescriptors) { Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)); } else { ownKeys(Object(source)).forEach(function (key) { Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key)); }); } } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function _toConsumableArray(arr) { return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread(); }

function _nonIterableSpread() { throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }

function _iterableToArray(iter) { if (typeof Symbol !== "undefined" && Symbol.iterator in Object(iter)) return Array.from(iter); }

function _arrayWithoutHoles(arr) { if (Array.isArray(arr)) return _arrayLikeToArray(arr); }

function _slicedToArray(arr, i) { return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _unsupportedIterableToArray(arr, i) || _nonIterableRest(); }

function _nonIterableRest() { throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }

function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }

function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) { arr2[i] = arr[i]; } return arr2; }

function _iterableToArrayLimit(arr, i) { if (typeof Symbol === "undefined" || !(Symbol.iterator in Object(arr))) return; var _arr = []; var _n = true; var _d = false; var _e = undefined; try { for (var _i = arr[Symbol.iterator](), _s; !(_n = (_s = _i.next()).done); _n = true) { _arr.push(_s.value); if (i && _arr.length === i) break; } } catch (err) { _d = true; _e = err; } finally { try { if (!_n && _i["return"] != null) _i["return"](); } finally { if (_d) throw _e; } } return _arr; }

function _arrayWithHoles(arr) { if (Array.isArray(arr)) return arr; }




 //var self = this;

function RADClientIDSecretApp(props) {
  var _useState = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)([]),
      _useState2 = _slicedToArray(_useState, 2),
      idList = _useState2[0],
      setIdList = _useState2[1];

  var _useState3 = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)({}),
      _useState4 = _slicedToArray(_useState3, 2),
      appInfo = _useState4[0],
      setAppInfo = _useState4[1];

  var _useState5 = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)(false),
      _useState6 = _slicedToArray(_useState5, 2),
      showCreateUserPopUp = _useState6[0],
      setShowCreateUserPopUp = _useState6[1];

  var _useState7 = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)(""),
      _useState8 = _slicedToArray(_useState7, 2),
      selectedUser = _useState8[0],
      setSelectedUser = _useState8[1];

  (0,react__WEBPACK_IMPORTED_MODULE_0__.useEffect)(function () {
    getIdList(props.serviceUrl).then(function (data) {
      setIdList(data.ClientIdSecretList || []);
      var appInfo = {};
      appInfo.Groups = data.Groups ? JSON.parse(data.Groups) : [];
      appInfo.Users = data.Users ? JSON.parse(data.Users) : [];
      appInfo.Roles = data.Roles ? JSON.parse(data.Roles) : [];
      appInfo.Accounts = data.Accounts ? JSON.parse(data.Accounts) : [];
      setAppInfo(appInfo);
    });
  }, []);

  function createNewClientIDSecret(info) {
    var clientIdInfo = {
      UserName: info.User,
      Description: info.Description,
      ExpiryTime: info.ExpiryYears + "|" + info.ExpiryMonths + "|" + info.ExpiryDays,
      NotificationMailIds: info.Notification
    };
    $.ajax({
      url: props.serviceUrl + '/Resources/Services/RADClientIDSecret.svc/SaveClientIDSecret',
      type: 'POST',
      contentType: "application/json",
      data: JSON.stringify({
        info: JSON.stringify(clientIdInfo)
      }),
      dataType: 'json'
    }).then(function (res) {
      if (res.d) {
        var list = _toConsumableArray(idList);

        var result = JSON.parse(res.d);
        result.NewId = true;
        list.push(result);
        setIdList(list);
      }
    });
  }

  function showHideCreateUserPopUp() {
    setShowCreateUserPopUp(!showCreateUserPopUp);
  }

  function createNewUser(data) {
    $.ajax({
      url: props.serviceUrl + '/Resources/Services/RADClientIDSecret.svc/CreateNewUser',
      type: 'POST',
      contentType: "application/json",
      data: JSON.stringify({
        userDetails: JSON.stringify(data)
      }),
      dataType: 'json'
    }).then(function (res) {
      if (res.d == "") {
        var newUser = data; // window.selectedUser = JSON.parse(JSON.stringify(newUser));

        setSelectedUser(newUser);
        setShowCreateUserPopUp(false);
        setAppInfo(_objectSpread(_objectSpread({}, appInfo), {}, {
          Users: [].concat(_toConsumableArray(appInfo.Users), [newUser])
        }));
      }
    });
  }

  function DeleteClientIDSecret(id) {
    $.ajax({
      url: props.serviceUrl + '/Resources/Services/RADClientIDSecret.svc/DeleteClientIDSecret',
      type: 'POST',
      contentType: "application/json",
      data: JSON.stringify({
        id: id
      }),
      dataType: 'json'
    }).then(function (res) {
      if (res.d) {
        var list = _toConsumableArray(idList);

        list.splice(list.findIndex(function (x) {
          return x.id == id;
        }), 1);
        setIdList(list);
      }
    });
  }

  function RenewClientID(id) {
    $.ajax({
      url: props.serviceUrl + '/Resources/Services/RADClientIDSecret.svc/RenewClientIDSecret',
      type: 'POST',
      contentType: "application/json",
      data: JSON.stringify({
        info: JSON.stringify(idList.find(function (x) {
          return x.id == id;
        }))
      }),
      dataType: 'json'
    }).then(function (res) {
      if (res.d) {
        var list = _toConsumableArray(idList);

        delete res.d.__type;
        var result = res.d;
        result.NewId = true;
        var index = list.findIndex(function (x) {
          return x.id == id;
        });
        list.splice(index, 1, result);
        setIdList(list);
      }
    });
  } // var user = null;
  // if (window.selectedUser) {
  //     user = window.selectedUser;
  //     window.selectedUser = null;
  // }


  return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    id: "RADClientIDSecretApp"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDSecretMainBody"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement(_createID__WEBPACK_IMPORTED_MODULE_1__.default, {
    Disabled: showCreateUserPopUp,
    selectedUser: selectedUser,
    Users: appInfo.Users,
    showHideCreateUserPopUp: showHideCreateUserPopUp,
    createNewClientIDSecret: createNewClientIDSecret
  }), showCreateUserPopUp && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement(_createNewUser__WEBPACK_IMPORTED_MODULE_3__.default, {
    showHideCreateUserPopUp: showHideCreateUserPopUp,
    createNewUser: createNewUser,
    Info: appInfo
  }), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement(_IdList__WEBPACK_IMPORTED_MODULE_2__.default, {
    RenewClientID: RenewClientID,
    DeleteClientIDSecret: DeleteClientIDSecret,
    idList: idList
  })));
}

function getIdList(url) {
  return new Promise(function (resolve, reject) {
    $.ajax({
      url: url + '/Resources/Services/RADClientIDSecret.svc/InitializeClientIDApp',
      type: 'POST',
      contentType: "application/json",
      dataType: 'json'
    }).then(function (res) {
      console.log(res);
      resolve(res.d);
    });
  });
}

/***/ }),

/***/ "./components/createID.js":
/*!********************************!*\
  !*** ./components/createID.js ***!
  \********************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => /* binding */ RADCreateID
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _dropdown__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./dropdown */ "./components/dropdown.js");
function ownKeys(object, enumerableOnly) { var keys = Object.keys(object); if (Object.getOwnPropertySymbols) { var symbols = Object.getOwnPropertySymbols(object); if (enumerableOnly) symbols = symbols.filter(function (sym) { return Object.getOwnPropertyDescriptor(object, sym).enumerable; }); keys.push.apply(keys, symbols); } return keys; }

function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i] != null ? arguments[i] : {}; if (i % 2) { ownKeys(Object(source), true).forEach(function (key) { _defineProperty(target, key, source[key]); }); } else if (Object.getOwnPropertyDescriptors) { Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)); } else { ownKeys(Object(source)).forEach(function (key) { Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key)); }); } } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function _slicedToArray(arr, i) { return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _unsupportedIterableToArray(arr, i) || _nonIterableRest(); }

function _nonIterableRest() { throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }

function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }

function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) { arr2[i] = arr[i]; } return arr2; }

function _iterableToArrayLimit(arr, i) { if (typeof Symbol === "undefined" || !(Symbol.iterator in Object(arr))) return; var _arr = []; var _n = true; var _d = false; var _e = undefined; try { for (var _i = arr[Symbol.iterator](), _s; !(_n = (_s = _i.next()).done); _n = true) { _arr.push(_s.value); if (i && _arr.length === i) break; } } catch (err) { _d = true; _e = err; } finally { try { if (!_n && _i["return"] != null) _i["return"](); } finally { if (_d) throw _e; } } return _arr; }

function _arrayWithHoles(arr) { if (Array.isArray(arr)) return arr; }



function RADCreateID(props) {
  var _useState = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)(false),
      _useState2 = _slicedToArray(_useState, 2),
      showForm = _useState2[0],
      setShowForm = _useState2[1];

  var _useState3 = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)(null),
      _useState4 = _slicedToArray(_useState3, 2),
      userDD = _useState4[0],
      setUserDD = _useState4[1]; //const [ExpDate, setExpDate] = useState(null);


  var _useState5 = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)({
    User: "",
    Description: "",
    ExpiryYears: 0,
    ExpiryMonths: 0,
    ExpiryDays: 0,
    Notification: ""
  }),
      _useState6 = _slicedToArray(_useState5, 2),
      formData = _useState6[0],
      setFormData = _useState6[1]; // useEffect(() => {
  //     $(".RADCreatIDExpInput").datepicker({ autoclose: true, format: 'mm/dd/yyyy' });
  //     $(".RADCreatIDExpInput").datepicker().on("changeDate", date => handleFormDataChange(date, "Exp"));
  //     $(".RADCreatIDExpInput").attr("onkeydown", "{return false;}");
  //     return () => {
  //         if ($(".RADCreatIDExpInput").data('datepicker')) {
  //             $(".RADCreatIDExpInput").data('datepicker').remove();
  //             $(".RADCreatIDExpInput").unbind();
  //         }
  //     }
  // }, [showForm]);


  function openUserDD(e) {
    if (userDD == null) setUserDD(e.currentTarget);
  }

  function handleDDSelect(option) {
    setFormData(_objectSpread(_objectSpread({}, formData), {}, {
      User: option
    }));
    setUserDD(null);
  }

  function handleFormDataChange(e, field) {
    switch (field) {
      case "Desc":
        setFormData(_objectSpread(_objectSpread({}, formData), {}, {
          Description: e.target.value
        }));
        break;

      case "Exp-Y":
        setFormData(_objectSpread(_objectSpread({}, formData), {}, {
          ExpiryYears: e.target.value
        }));
        break;

      case "Exp-M":
        setFormData(_objectSpread(_objectSpread({}, formData), {}, {
          ExpiryMonths: e.target.value
        }));
        break;

      case "Exp-D":
        setFormData(_objectSpread(_objectSpread({}, formData), {}, {
          ExpiryDays: e.target.value
        }));
        break;

      case "Notification":
        setFormData(_objectSpread(_objectSpread({}, formData), {}, {
          Notification: e.target.value
        }));
        break;
    }
  }

  function closeForm() {
    setFormData({
      User: "",
      Description: "",
      ExpiryYears: 0,
      ExpiryMonths: 0,
      ExpiryDays: 0,
      Notification: ""
    });
    setShowForm(false);
  }

  function CreateNewClientSecretID() {
    if (validateInfo()) {
      props.createNewClientIDSecret(formData);
      closeForm();
    }
  }

  function validateInfo() {
    if (formData.User != "" && (formData.ExpiryDays != 0 || formData.ExpiryMonths != 0 || formData.ExpiryYears != 0)) return true;
    return false;
  }

  if (props.selectedUser && formData.User == "") {
    setFormData(_objectSpread(_objectSpread({}, formData), {}, {
      User: props.selectedUser.UserLoginName
    }));
  }

  return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: props.Disabled ? "RADCreateIDMain RADCreateIDMainDisabled" : "RADCreateIDMain"
  }, !showForm && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: function onClick() {
      return setShowForm(true);
    },
    className: "RADClientAppButton RADCreateIDButton"
  }, " +  Create New ID"), showForm && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDHeader"
  }, "Create new client Id"), showForm && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDBody"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDFieldsParent"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDField RADCreateIDFieldUser"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateIDFieldLabel form-label"
  }, "User"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    style: {
      display: "flex"
    }
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: openUserDD,
    style: {
      position: "relative",
      width: 'calc(100% - 35px)'
    }
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDFieldInput RADCreateIDUserDD"
  }, formData.User), Boolean(userDD) && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement(_dropdown__WEBPACK_IMPORTED_MODULE_1__.default, {
    EnableSearch: true,
    values: props.Users.map(function (x) {
      return x.UserLoginName;
    }),
    anchorEle: userDD,
    handleClose: handleDDSelect
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: props.showHideCreateUserPopUp,
    className: "RADCreateNewUserOption"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("i", {
    "class": "fas fa-user-plus"
  })))), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDField RADCreateIDFieldDesc"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateIDFieldLabel form-label"
  }, "Description"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("input", {
    onChange: function onChange(e) {
      return handleFormDataChange(e, "Desc");
    },
    value: formData.Description,
    className: "RADCreateIDFieldInput RADCreatIDDescInput"
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDField RADCreateIDFieldExpiry"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateIDFieldLabel form-label"
  }, "Expiry"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDExpiryInputParent"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDExpiryInput RADCreateIDExpiryInputYears"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateIDFieldLabel form-label"
  }, "Years"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("input", {
    type: "Number",
    onChange: function onChange(e) {
      return handleFormDataChange(e, "Exp-Y");
    },
    value: formData.ExpiryYears,
    className: "RADCreateIDFieldInput RADCreatIDDescInput"
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDExpiryInput RADCreateIDExpiryInputMonths"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateIDFieldLabel form-label"
  }, "Months"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("input", {
    type: "Number",
    onChange: function onChange(e) {
      return handleFormDataChange(e, "Exp-M");
    },
    value: formData.ExpiryMonths,
    className: "RADCreateIDFieldInput RADCreatIDDescInput"
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDExpiryInput RADCreateIDExpiryInputDays"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateIDFieldLabel form-label"
  }, "Days"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("input", {
    type: "Number",
    onChange: function onChange(e) {
      return handleFormDataChange(e, "Exp-D");
    },
    value: formData.ExpiryDays,
    className: "RADCreateIDFieldInput RADCreatIDDescInput"
  })))), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDField RADCreateIDFieldNotification"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("textarea", {
    "aria-label": "Notification",
    placeholder: "Notification",
    onChange: function onChange(e) {
      return handleFormDataChange(e, "Notification");
    },
    className: "RADCreateIDFieldInput RADCreatIDNotificationInput"
  }))), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateIDActionParent"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: CreateNewClientSecretID,
    className: "RADClientAppButton",
    id: "RADCreateIDDone"
  }, "Done"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: closeForm,
    className: "RADClientAppButton2",
    id: "RADCreateIDCancel"
  }, "Cancel"))));
}

/***/ }),

/***/ "./components/createNewUser.js":
/*!*************************************!*\
  !*** ./components/createNewUser.js ***!
  \*************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => /* binding */ RADCreateNewUser
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _dropdown__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./dropdown */ "./components/dropdown.js");
function ownKeys(object, enumerableOnly) { var keys = Object.keys(object); if (Object.getOwnPropertySymbols) { var symbols = Object.getOwnPropertySymbols(object); if (enumerableOnly) symbols = symbols.filter(function (sym) { return Object.getOwnPropertyDescriptor(object, sym).enumerable; }); keys.push.apply(keys, symbols); } return keys; }

function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i] != null ? arguments[i] : {}; if (i % 2) { ownKeys(Object(source), true).forEach(function (key) { _defineProperty(target, key, source[key]); }); } else if (Object.getOwnPropertyDescriptors) { Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)); } else { ownKeys(Object(source)).forEach(function (key) { Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key)); }); } } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function _slicedToArray(arr, i) { return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _unsupportedIterableToArray(arr, i) || _nonIterableRest(); }

function _nonIterableRest() { throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }

function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }

function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) { arr2[i] = arr[i]; } return arr2; }

function _iterableToArrayLimit(arr, i) { if (typeof Symbol === "undefined" || !(Symbol.iterator in Object(arr))) return; var _arr = []; var _n = true; var _d = false; var _e = undefined; try { for (var _i = arr[Symbol.iterator](), _s; !(_n = (_s = _i.next()).done); _n = true) { _arr.push(_s.value); if (i && _arr.length === i) break; } } catch (err) { _d = true; _e = err; } finally { try { if (!_n && _i["return"] != null) _i["return"](); } finally { if (_d) throw _e; } } return _arr; }

function _arrayWithHoles(arr) { if (Array.isArray(arr)) return arr; }



function RADCreateNewUser(props) {
  var _useState = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)(null),
      _useState2 = _slicedToArray(_useState, 2),
      AccountDDcontrol = _useState2[0],
      setAccountDDcontrol = _useState2[1];

  var _useState3 = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)(null),
      _useState4 = _slicedToArray(_useState3, 2),
      GroupDDcontrol = _useState4[0],
      setGroupDDcontrol = _useState4[1];

  var _useState5 = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)(null),
      _useState6 = _slicedToArray(_useState5, 2),
      RoleDDcontrol = _useState6[0],
      setRoleDDcontrol = _useState6[1];

  var _useState7 = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)({
    UserLoginName: "",
    FirstName: "",
    LastName: "",
    EmailId: "",
    Groups: "",
    Roles: "",
    Accounts: ""
  }),
      _useState8 = _slicedToArray(_useState7, 2),
      createUserFormData = _useState8[0],
      setCreateUserFormData = _useState8[1];

  function openDD(e, DD) {
    if (DD == "Account") setAccountDDcontrol(e.currentTarget);else if (DD == "Group") setGroupDDcontrol(e.currentTarget);else if (DD == "Role") setRoleDDcontrol(e.currentTarget);
  }

  function validateInfo() {
    return true;
  }

  function createNewUser() {
    if (validateInfo()) {
      var info = JSON.parse(JSON.stringify(createUserFormData));
      info.Groups = info.Groups.split(",");
      info.Roles = info.Roles.split(",");
      props.createNewUser(info);
    }
  }

  function handleFormData(data, field) {
    switch (field) {
      case "UserName":
        setCreateUserFormData(_objectSpread(_objectSpread({}, createUserFormData), {}, {
          UserLoginName: data
        }));
        break;

      case "FirstName":
        setCreateUserFormData(_objectSpread(_objectSpread({}, createUserFormData), {}, {
          FirstName: data
        }));
        break;

      case "LastName":
        setCreateUserFormData(_objectSpread(_objectSpread({}, createUserFormData), {}, {
          LastName: data
        }));
        break;

      case "Email":
        setCreateUserFormData(_objectSpread(_objectSpread({}, createUserFormData), {}, {
          EmailId: data
        }));
        break;

      case "Groups":
        data && setCreateUserFormData(_objectSpread(_objectSpread({}, createUserFormData), {}, {
          Groups: data.join(",")
        }));
        setGroupDDcontrol(null);
        break;

      case "Roles":
        data && setCreateUserFormData(_objectSpread(_objectSpread({}, createUserFormData), {}, {
          Roles: data.join(",")
        }));
        setRoleDDcontrol(null);
        break;

      case "Accounts":
        data && setCreateUserFormData(_objectSpread(_objectSpread({}, createUserFormData), {}, {
          Accounts: [data]
        }));
        setAccountDDcontrol(null);
        break;
    }
  }

  return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateNewUserMain"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateNewUserHeader"
  }, "Create new user"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateNewUserBody"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserForm"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormField RADCreateUserFormFieldUserName"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateUserFormFieldLabel form-label"
  }, "User Name"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("input", {
    onChange: function onChange(e) {
      return handleFormData(e.target.value, "UserName");
    },
    value: createUserFormData.UserName,
    className: "RADCreateUserFormFieldInput"
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormField RADCreateUserFormFieldFirstName"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateUserFormFieldLabel form-label"
  }, "First Name"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("input", {
    onChange: function onChange(e) {
      return handleFormData(e.target.value, "FirstName");
    },
    value: createUserFormData.FirstName,
    className: "RADCreateUserFormFieldInput"
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormField RADCreateUserFormFieldLastName"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateUserFormFieldLabel form-label"
  }, "Last Name"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("input", {
    onChange: function onChange(e) {
      return handleFormData(e.target.value, "LastName");
    },
    value: createUserFormData.LastName,
    className: "RADCreateUserFormFieldInput"
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormField RADCreateUserFormFieldEmail"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateUserFormFieldLabel form-label"
  }, "Email ID"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("input", {
    onChange: function onChange(e) {
      return handleFormData(e.target.value, "Email");
    },
    value: createUserFormData.Email,
    className: "RADCreateUserFormFieldInput"
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormField RADCreateUserFormFieldAccounts"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateUserFormFieldLabel form-label"
  }, "Accounts"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: function onClick(e) {
      return openDD(e, "Account");
    },
    className: "RADCreateUserFormFieldDDParent"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormFieldDD"
  }, createUserFormData.Accounts), Boolean(AccountDDcontrol) && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement(_dropdown__WEBPACK_IMPORTED_MODULE_1__.default, {
    EnableSearch: true,
    values: props.Info.Accounts.map(function (x) {
      return x.AccountName;
    }),
    anchorEle: AccountDDcontrol,
    handleClose: function handleClose(e) {
      return handleFormData(e, "Accounts");
    }
  }))), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormField RADCreateUserFormFieldGroups"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateUserFormFieldLabel form-label"
  }, "Groups"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: function onClick(e) {
      return openDD(e, "Group");
    },
    className: "RADCreateUserFormFieldDDParent"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormFieldDD"
  }, createUserFormData.Groups), Boolean(GroupDDcontrol) && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement(_dropdown__WEBPACK_IMPORTED_MODULE_1__.default, {
    EnableSearch: true,
    selectedList: createUserFormData.Groups != "" ? createUserFormData.Groups.split(',') : [],
    multiselect: true,
    values: props.Info.Groups.map(function (x) {
      return x.GroupName;
    }),
    anchorEle: GroupDDcontrol,
    handleClose: function handleClose(e) {
      return handleFormData(e, "Groups");
    }
  }))), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormField RADCreateUserFormFieldRoles"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("label", {
    className: "RADCreateUserFormFieldLabel form-label"
  }, "Roles"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: function onClick(e) {
      return openDD(e, "Role");
    },
    className: "RADCreateUserFormFieldDDParent"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateUserFormFieldDD"
  }, createUserFormData.Roles), Boolean(RoleDDcontrol) && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement(_dropdown__WEBPACK_IMPORTED_MODULE_1__.default, {
    EnableSearch: true,
    multiselect: true,
    selectedList: createUserFormData.Roles != "" ? createUserFormData.Roles.split(',') : [],
    values: props.Info.Roles.map(function (x) {
      return x.RoleName;
    }),
    anchorEle: RoleDDcontrol,
    handleClose: function handleClose(e) {
      return handleFormData(e, "Roles");
    }
  })))), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADCreateNewUserActionBar"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: createNewUser,
    className: "RADClientAppButton RADClientAppDoneButton"
  }, "Done"), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    onClick: props.showHideCreateUserPopUp,
    className: "RADClientAppButton2"
  }, "Cancel"))));
}

/***/ }),

/***/ "./components/dropdown.js":
/*!********************************!*\
  !*** ./components/dropdown.js ***!
  \********************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => /* binding */ RADClientSecretDropDown
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
function _toConsumableArray(arr) { return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread(); }

function _nonIterableSpread() { throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }

function _iterableToArray(iter) { if (typeof Symbol !== "undefined" && Symbol.iterator in Object(iter)) return Array.from(iter); }

function _arrayWithoutHoles(arr) { if (Array.isArray(arr)) return _arrayLikeToArray(arr); }

function _slicedToArray(arr, i) { return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _unsupportedIterableToArray(arr, i) || _nonIterableRest(); }

function _nonIterableRest() { throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }

function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }

function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) { arr2[i] = arr[i]; } return arr2; }

function _iterableToArrayLimit(arr, i) { if (typeof Symbol === "undefined" || !(Symbol.iterator in Object(arr))) return; var _arr = []; var _n = true; var _d = false; var _e = undefined; try { for (var _i = arr[Symbol.iterator](), _s; !(_n = (_s = _i.next()).done); _n = true) { _arr.push(_s.value); if (i && _arr.length === i) break; } } catch (err) { _d = true; _e = err; } finally { try { if (!_n && _i["return"] != null) _i["return"](); } finally { if (_d) throw _e; } } return _arr; }

function _arrayWithHoles(arr) { if (Array.isArray(arr)) return arr; }


function RADClientSecretDropDown(_ref) {
  var anchorEle = _ref.anchorEle,
      _ref$selectedList = _ref.selectedList,
      selectedList = _ref$selectedList === void 0 ? [] : _ref$selectedList,
      handleClose = _ref.handleClose,
      values = _ref.values,
      _ref$EnableSearch = _ref.EnableSearch,
      EnableSearch = _ref$EnableSearch === void 0 ? false : _ref$EnableSearch,
      _ref$multiselect = _ref.multiselect,
      multiselect = _ref$multiselect === void 0 ? false : _ref$multiselect;
  var ddRef = (0,react__WEBPACK_IMPORTED_MODULE_0__.useRef)(null);

  var _useState = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)(""),
      _useState2 = _slicedToArray(_useState, 2),
      text = _useState2[0],
      setText = _useState2[1];

  var _useState3 = (0,react__WEBPACK_IMPORTED_MODULE_0__.useState)(selectedList),
      _useState4 = _slicedToArray(_useState3, 2),
      selectList = _useState4[0],
      setSelectList = _useState4[1];

  (0,react__WEBPACK_IMPORTED_MODULE_0__.useEffect)(function () {
    function handleOutsideClick(e) {
      if (e.target.className == "RADClientIDDropDownListSearch") return;

      if (multiselect) {
        if (anchorEle && anchorEle.contains(e.target) && ddRef.current.contains(e.target)) {
          if (selectList.includes(e.target.innerText)) setSelectList(selectList.filter(function (x) {
            return x != e.target.innerText;
          }));else setSelectList([].concat(_toConsumableArray(selectList), [e.target.innerText]));
        } else handleClose(selectList);
      } else {
        if (anchorEle && anchorEle.contains(e.target) && ddRef.current.contains(e.target)) handleClose(e.target.innerText);else handleClose(null);
      }
    }

    document.addEventListener("click", handleOutsideClick);
    return function () {
      document.removeEventListener("click", handleOutsideClick);
    };
  }, [anchorEle, selectList]);

  var list = _toConsumableArray(values);

  if (EnableSearch) list = list.filter(function (x) {
    return x.toUpperCase().indexOf(text.toUpperCase()) != -1;
  });
  if (multiselect) list = list.map(function (x) {
    if (selectList.includes(x)) return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      title: x,
      className: "RADClientIDDropDownListOption RADClientIDDDOptionSelected"
    }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("span", {
      className: "RADClientIDDDSelectIcon fas fa-check"
    }), x);else return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      title: x,
      className: "RADClientIDDropDownListOption"
    }, x);
  });else list = list.map(function (x) {
    return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
      title: x,
      className: "RADClientIDDropDownListOption"
    }, x);
  });
  return /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    ref: ddRef,
    className: "RADClientIDDropDownList"
  }, EnableSearch && /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDDropDownListSearchParent"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("span", {
    className: "RADClientIDDropDownListSearchIcon"
  }, /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("i", {
    "class": "fas fa-search"
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("input", {
    className: "RADClientIDDropDownListSearch",
    onChange: function onChange(e) {
      return setText(e.target.value);
    }
  })), /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement("div", {
    className: "RADClientIDDropDownListBody"
  }, list));
}

/***/ }),

/***/ "./index.js":
/*!******************!*\
  !*** ./index.js ***!
  \******************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => __WEBPACK_DEFAULT_EXPORT__
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var react_dom__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! react-dom */ "react-dom");
/* harmony import */ var react_dom__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(react_dom__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _components_app_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./components/app.js */ "./components/app.js");




var RADClientIDSecretModule = function (props) {
  function getBaseUrl() {
    var re = /.*Home.aspx/gi;
    var url = window.location.href;
    var m = re.exec(url);
    if (m === null) return url;else return m[0].replace("/Home.aspx", "");
  }

  function initialize(_ref) {
    var divId = _ref.divId,
        serviceUrl = _ref.serviceUrl;
    react_dom__WEBPACK_IMPORTED_MODULE_1___default().render( /*#__PURE__*/react__WEBPACK_IMPORTED_MODULE_0___default().createElement(_components_app_js__WEBPACK_IMPORTED_MODULE_2__.default, {
      serviceUrl: serviceUrl || getBaseUrl()
    }), document.getElementById(divId));
  }

  return {
    initialize: initialize
  };
}();

/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (RADClientIDSecretModule);

/***/ }),

/***/ "react":
/*!************************!*\
  !*** external "React" ***!
  \************************/
/***/ ((module) => {

module.exports = React;

/***/ }),

/***/ "react-dom":
/*!***************************!*\
  !*** external "ReactDOM" ***!
  \***************************/
/***/ ((module) => {

module.exports = ReactDOM;

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		if(__webpack_module_cache__[moduleId]) {
/******/ 			return __webpack_module_cache__[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/compat get default export */
/******/ 	(() => {
/******/ 		// getDefaultExport function for compatibility with non-harmony modules
/******/ 		__webpack_require__.n = (module) => {
/******/ 			var getter = module && module.__esModule ?
/******/ 				() => module['default'] :
/******/ 				() => module;
/******/ 			__webpack_require__.d(getter, { a: getter });
/******/ 			return getter;
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => Object.prototype.hasOwnProperty.call(obj, prop)
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
/******/ 	// module exports must be returned from runtime so entry inlining is disabled
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__("./index.js");
/******/ })()
.default;
//# sourceMappingURL=RADClientIDSecretModule.js.map