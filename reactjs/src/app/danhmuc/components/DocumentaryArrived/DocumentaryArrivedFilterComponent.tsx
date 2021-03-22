import { reaction } from 'mobx';
import React, { Component } from 'react'
import { JarvisWidget } from '../../../../common'
import { DocumentaryType } from '../../../../common/core/models/Attachment';
import { JavisWidgetDefault } from '../../../../common/core/models/JavisDefault';
import { isGranted } from '../../../../lib/abpUtility';
import { PermissionNames } from '../../../../lib/PermissionName';

export interface IDocumentaryArrivedFilterProps {
    onSearch?: (filterData: any) => any
}

export default class DocumentaryArrivedFilterComponent extends Component<IDocumentaryArrivedFilterProps, any> {
    constructor(props: any) {
        super(props);
        const years = [];
        const currentYear = new Date().getFullYear();
        for (var year = currentYear; year > (currentYear - 10); year--) {
            years.push(year)
        }
        this.state = {
            filterData: {
                keyword: '',
                filterBy: 1,
                exactly: false,
                type: DocumentaryType.DocumentaryArrived,
                approved: 0,
                year: currentYear
            },
            years: years
        }
        this.handleInputChange = this.handleInputChange.bind(this);
        this.handleSearch = this.handleSearch.bind(this);
    }
    handleSearch() {
        if (this.props.onSearch !== undefined) {
            this.props.onSearch(this.state.filterData);
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
    render() {
        const { years } = this.state;
        const allowApproved = isGranted(PermissionNames.Permission_Approved);
        const allowDocument = isGranted(PermissionNames.Permission_DocumentManager);
        return (
            <JarvisWidget id="wid-id-filter-van-ban-den" editbutton={false} color={JavisWidgetDefault.color} refresh={true}>
                <header>
                    <span className="widget-icon">
                        <i className="fa fa-search" />
                    </span>
                    <h2>Tìm kiếm</h2>
                </header>
                <div>
                    <div className="widget-body" style={{ minHeight: 'unset' }}>
                        <div className="form-horizontal form-custom">
                            <div className="col-lg-3 col-md-6">
                                <div className="form-group">

                                    <label htmlFor="inputEmail3" className="col-sm-2 control-label">Từ khóa</label>
                                    <div className="col-sm-10">
                                        <input className="form-control" onChange={this.handleInputChange} value={this.state.filterData.keyword} placeholder="Nhập từ khóa cần tìm kiếm..." name="keyword" type="text"></input>
                                    </div>
                                </div>
                            </div>
                            <div className="col-lg-3 col-md-6">
                                <div className="form-group">
                                    <label htmlFor="inputEmail3" className="col-sm-2 control-label">Duyệt</label>
                                    <div className="col-sm-10">
                                        <select className="form-control" value={this.state.filterData.approved} id="approved" name="approved"
                                            onChange={this.handleInputChange}>
                                            <option value="0">Tất cả</option>
                                            <option value="1">Đã duyệt</option>
                                            <option value="2">Chưa duyệt</option>
                                        </select>
                                    </div>
                                </div>
                            </div>

                            <div className="dynamic-clear"></div>
                            <div className="col-lg-3 col-md-6">
                                <div className="form-group">
                                    <div className="col-lg-12 col-lg-offset-0 col-md-10 col-md-offset-2  col-sm-10 col-sm-offset-2">
                                        <div className="form-control">
                                            <label className="radio-inline" style={{ width: '47%' }}>
                                                <input onChange={this.handleInputChange} checked={this.state.filterData.filterBy == '1'} type="radio" value="1" name="filterBy"></input>Theo ký hiệu
                                                            </label>
                                            <label className="radio-inline">
                                                <input onChange={this.handleInputChange} checked={this.state.filterData.filterBy == '2'} type="radio" value="2" name="filterBy"></input>Theo tóm tắt
                                                            </label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="col-lg-2 col-md-6">
                                <div className="form-group">
                                    <div>
                                        <label htmlFor="inputEmail3" className="col-sm-4 control-label">Năm</label>
                                        <div className="col-sm-8">
                                            <select className="form-control" onChange={this.handleInputChange} value={this.state.filterData.year} placeholder="Năm..." name="year">
                                                <option key='all' value={-1} >Tất cả</option>
                                                {years.map((year: number) => (<option key={year} value={year}>{year}</option>))}
                                            </select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div style={{ display: 'none' }} className="col-lg-2 col-md-6">
                                <div className="form-group">
                                    <div className="col-lg-12 col-lg-offset-0 col-md-10 col-md-offset-2 col-sm-10 col-sm-offset-2">
                                        <label className="checkbox-inline">
                                            <input
                                                onChange={this.handleInputChange}
                                                checked={this.state.filterData.exactly}
                                                type="checkbox" name="exactly"
                                            ></input>
                                                Tìm chính xác
                                                </label>
                                    </div>
                                </div>
                            </div>
                            <div style={{ clear: 'both' }}></div>

                            <div className="col-lg-12 col-md-12">
                                <div className="form-group">
                                    <div className="col-sm-12 text-right">
                                        <button type="button" onClick={this.handleSearch} className="btn btn-primary"><i className="fa fa-search"></i>&nbsp;Tìm kiếm</button>
                                    </div>
                                </div>
                            </div>



                            {/* <div className="form-group">
                                <label className="control-label col-md-1">Từ khóa:&nbsp;&nbsp;</label>
                                <div className={(allowApproved || allowDocument) ? ("col-md-2") : "col-md-5"}>
                                    <input className="form-control" onChange={this.handleInputChange} value={this.state.filterData.keyword} placeholder="Nhập từ khóa cần tìm kiếm..." name="keyword" type="text"></input>
                                </div>
                                {
                                    (allowApproved || allowDocument) ? (
                                        <React.Fragment>
                                            <label className="control-label col-md-1">Duyệt:&nbsp;&nbsp;</label>
                                            <div className="col-md-2">
                                                <select className="form-control" value={this.state.filterData.approved} id="approved" name="approved"
                                                    onChange={this.handleInputChange}>
                                                    <option value="0">Tất cả</option>
                                                    <option value="1">Đã duyệt</option>
                                                    <option value="2">Chưa duyệt</option>
                                                </select>
                                            </div>
                                        </React.Fragment>
                                    ) : null
                                }

                                <div className="col-md-3">
                                    <div className="form-control">
                                        <label className="radio-inline" style={{ width: '47%' }}>
                                            <input onChange={this.handleInputChange} checked={this.state.filterData.filterBy == '1'} type="radio" value="1" name="filterBy"></input>Theo ký hiệu
                                                            </label>
                                        <label className="radio-inline">
                                            <input onChange={this.handleInputChange} checked={this.state.filterData.filterBy == '2'} type="radio" value="2" name="filterBy"></input>Theo tóm tắt
                                                            </label>
                                    </div>
                                </div>
                                <div className="col-md-2">
                                    <label className="checkbox-inline">
                                        <input onChange={this.handleInputChange} checked={this.state.filterData.exactly} type="checkbox" name="exactly"></input>Tìm chính xác</label>
                                </div>
                                <div className="col-md-1">
                                    <button type="button" onClick={this.handleSearch} className="btn btn-primary"><i className="fa fa-search"></i>&nbsp;Tìm kiếm</button>
                                </div>
                            </div>
                         */}
                        </div>
                    </div>
                </div>
            </JarvisWidget>
        )
    }
}
