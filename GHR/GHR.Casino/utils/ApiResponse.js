class ApiResponse {
  constructor({ success = true, message = '', data = null, error = null }) {
    this.success = success;
    this.message = message;
    this.data = data;
    this.error = error;
  }
}

module.exports = ApiResponse;
