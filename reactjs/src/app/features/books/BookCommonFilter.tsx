import React, { Component } from 'react'
import Select from 'react-select';
import { JarvisWidget } from '../../../common';
import CommonSelect from '../../../common/core/controls/CommonSelect';
import { JavisWidgetDefault } from '../../../common/core/models/JavisDefault';
import UiDatepicker from '../../../common/ui/components/jquery/UiDatepicker';
import DocumentTypeService from '../../../services/danhmuc/document-type/DocumentTypeService';
import { DocumentTypeDto } from '../../../services/danhmuc/document-type/dto/DocumentTypeDto';
import DocumentaryService from '../../../services/danhmuc/documentary/DocumentaryService';
export interface IBookCommonFilterProps {
    onSearch?: (filterData: any) => any,
    useTemplate: boolean,
    type: number,
    onPrint?: () => any
}

export default class BookCommonFilter extends Component<IBookCommonFilterProps, any> {
    constructor(props: any) {
        super(props); const years = [];
        const currentYear = new Date().getFullYear();
        for (var year = currentYear; year > (currentYear - 10); year--) {
            years.push(year)
        }
        this.state = {
            filterData: {
                year: new Date().getFullYear(),
                type: this.props.type,
                ngayTu: '',
                ngayDen: '',
                loaiVanBan: '',
                loaiVanBan_Name: ''
            },
            documentTypeOption: [],
            years: years
        }
        this.handleSelectChange = this.handleSelectChange.bind(this);
        this.handleSearch = this.handleSearch.bind(this);
        this.handlePrinting = this.handlePrinting.bind(this);
        this.handleDateChange = this.handleDateChange.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    componentDidMount() {
        DocumentTypeService.getPaging(0, 99999999).then(res => {
            const options = res.items.map((item: DocumentTypeDto) => {
                return {
                    value: item.id,
                    label: item.name
                }
            });
            this.setState({
                documentTypeOption: options
            })
        })
    }

    handleSearch() {
        if (this.props.onSearch !== undefined) {
            this.props.onSearch(this.state.filterData);
        }
    }

    handleSelectChange(e: any) {
        let filter = this.state.filterData || {};
        let name = e.target.name;
        let value = e.target.value;
        filter[name] = value;
        this.setState({
            filterData: filter,
        });
    }

    handlePrinting() {
        if (this.props.onPrint !== undefined) {
            this.props.onPrint();
        }
    }

    public getData() {
        return this.state.filterData;
    }
    public handleInputChange(e: any) {
        let filter = this.state.filterData || {};
        let name = e.target.name;
        let type = e.target.type;
        let value = type === "checkbox" ? e.target.checked : e.target.value;
        filter[name] = value;
        this.setState({
            filterData: filter,
        });
    }
    handleDateChange(fieldname: string, selectedDate: string) {
        let model = this.state.filterData || {};
        model[fieldname] = selectedDate;
        this.setState({
            filterData: model,
        });
    }
    handleCommonSelectChange(fieldName: string, selectedItem: any) {
        let model = this.state.filterData || {};
        if (selectedItem) {
            model[fieldName] = selectedItem.value;
            model[fieldName + '_Name'] = selectedItem.label;
        } else {
            model[fieldName] = '';
            model[fieldName + '_Name'] = '';
        }
        this.setState({
            filterData: model,
        });
    }
    render() {
        const { years } = this.state;
        return (
            <JarvisWidget id="wid-id-search-van-ban-di" editbutton={false} color={JavisWidgetDefault.color} refresh={true}>
                <header>
                    <span className="widget-icon">
                        <i className="fa fa-search" />
                    </span>
                    <h2>Tìm kiếm</h2>
                </header>
                <div>
                    <div className="widget-body" style={{ minHeight: 'unset' }}>
                        <div className="form-horizontal">
                            <div className="form-group">
                                <label className="control-label col-md-1">Ngày từ:</label>
                                <div className="col-md-3">
                                    <UiDatepicker
                                        dateFormat="dd/mm/yy"
                                        className="form-control"
                                        type="text"
                                        value={this.state.filterData.ngayTu}
                                        name="ngayTu"
                                        minRestrict="#ngayDen"
                                        id="ngayTu"
                                        placeholder="dd/MM/yyyy"
                                        onChange={this.handleDateChange}
                                    />
                                </div>
                                <label className="control-label col-md-1" >Ngày đến:</label>
                                <div className="col-md-3">
                                    <UiDatepicker
                                        dateFormat="dd/mm/yy"
                                        className="form-control"
                                        type="text"
                                        value={this.state.filterData.ngayDen}
                                        name="ngayDen"
                                        maxRestrict="#ngayTu"
                                        id="ngayDen"
                                        placeholder="dd/MM/yyyy"
                                        onChange={this.handleDateChange}
                                    />
                                </div>
                                <label className="control-label col-md-1">Loại văn bản:</label>
                                <div className="col-md-3">
                                    <CommonSelect
                                        isClearable={true}
                                        value={this.state.loaiVanBan}
                                        label={this.state.loaiVanBan_Name}
                                        options={this.state.documentTypeOption}
                                        clear
                                        fieldName="loaiVanBan"
                                        onChange={this.handleCommonSelectChange.bind(this)}
                                    >
                                    </CommonSelect>
                                </div>
                            </div>
                            <div className="form-group">
                                <label className="control-label col-md-1">Năm:</label>
                                <div className="col-md-3">
                                    <select className="form-control" onChange={this.handleInputChange} value={this.state.filterData.year} placeholder="Năm..." name="year">
                                        <option key='all' value={-1} >Tất cả</option>
                                        {years.map((year: number) => (<option key={year} value={year}>{year}</option>))}
                                    </select>
                                </div>
                                <div className="col-md-4 col-md-offset-4 text-right">
                                    <button type="button" style={{ marginLeft: '10px' }} onClick={this.handleSearch} className="btn btn-primary"><i className="fa fa-search"></i>&nbsp;Tìm kiếm</button>
                                    <button type="button" style={{ marginLeft: '10px' }} onClick={this.handlePrinting} className="btn btn-info"><i className="fa fa-print"></i>&nbsp;In</button>
                                </div>
                            </div>

                            {/* <div className="form-inline" style={this.props.useTemplate === true ? {} : { padding: '13px' }}>
                            <div className="form-group">
                                <label>Năm:&nbsp;</label>
                                <select onChange={this.handleSelectChange} name="year" className="form-control" style={{ width: '120px' }}>
                                    {
                                        source.map(item => {
                                            return <option key={item} value={item}>{item}</option>
                                        })
                                    }
                                </select>
                            </div>
                            <button type="button" style={{ marginLeft: '10px' }} onClick={this.handleSearch} className="btn btn-primary"><i className="fa fa-search"></i>&nbsp;Tìm</button>
                            <button type="button" style={{ marginLeft: '10px' }} onClick={this.handlePrinting} className="btn btn-info"><i className="fa fa-print"></i>&nbsp;In</button>
                        </div> */}
                        </div>
                    </div>
                </div>
            </JarvisWidget>
        )
    }
}
